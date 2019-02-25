using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class ArtificialIntelligence : MonoBehaviour
{
    enum AIState { PATROLLING, INVESTIGATING, CHASE }; // States
    AIState state;

    public Transform playerTarget;
    public GameObject playerHighlight, suspicious, alert;
    public Transform noisySource, stationeryPosition;
    public LayerMask layerMask;
    [Space]
    [Space]
    public AIPath aiPath;
    public float maxRadius, maxRadius2, maxRadius3, maxAngle, maxAngle2, maxAngle3, rotatingSpeed, walkSpeed, runSpeed, timeToStare, stopToGoBack, stopHere;
    //time to start = determines how long does the thug stare at YY/noise source before approaching, stopToGoBack = determines how long does the thug investigates before returning
    //maxRadius && maxAngle = suspicious FOV, //maxRadius2 && maxAngle2 = danger FOV, maxRadius3 && maxAngle3 = FOV to detect players running behind
    public int stopDest;
    public bool stationery, staticRotate, patrolTurn, followingLaoDa;
    [Space]
    [Space]
    [HideInInspector]
    public GameObject EmptyObj, questionMark, exclamationMark, firstFov, secondFov;
    [HideInInspector]
    public Vector3 lookHereStart;
     [HideInInspector]
    public int isInFov, investigatingState;
    [HideInInspector]
    public bool spottedHighlight, goToNoisySource;
    NavMeshAgent agent;
    Animator anim;
    public AudioSource[] externalAudio;
    Transform target, thisAI, uiAbove;
    Vector3 targetDir, newDir, directionBetween;
    Image uiState;
    PlayerLogic playerLogic;
    FaderLogic faderLogic;
    BGMControl bgmLogic;
    int destPoint = 0,randomIdle, randomStop, timesHitRotation;
    float stopToLook, angle, startToTurn, timeToResetView, currentAngle1;
    bool turnBack, cannotTurn, playerWithinRadius, dontMove;
    public string fileName, fileName2;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        thisAI = GetComponent<Transform>();
        externalAudio = GetComponents<AudioSource>();
        playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();
        bgmLogic = GameObject.Find("BGMS").GetComponent<BGMControl>();
        if (stationery)
        {
            EmptyObj = new GameObject("Look Here");
            EmptyObj.transform.parent = this.gameObject.transform;
            stationeryPosition = this.gameObject.transform.GetChild(6);
            if (!followingLaoDa) //spawns the guarding position on this ai
            {
                EmptyObj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                EmptyObj.transform.parent = null;
            }
            else if (followingLaoDa) //spawns the guarding position onto lao da to ensure this AI always follows him
            {
                EmptyObj.transform.position = new Vector3(GameObject.Find("Lao_Dav2").transform.position.x + 3, GameObject.Find("Lao_Dav2").transform.position.y, GameObject.Find("Lao_Dav2").transform.position.z);
                EmptyObj.transform.parent = GameObject.Find("Lao_Dav2").transform;
            }
            lookHereStart = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
            timeToResetView = 0.5f;
        }
        uiAbove = this.gameObject.transform.GetChild(5);
        questionMark = Instantiate(suspicious, transform.position, Quaternion.identity);
        exclamationMark = Instantiate(alert, transform.position, Quaternion.identity);
        questionMark.transform.parent = uiAbove;
        exclamationMark.transform.parent = uiAbove;
        questionMark.transform.position = new Vector3(uiAbove.position.x, uiAbove.position.y, uiAbove.position.z);
        exclamationMark.transform.position = new Vector3(uiAbove.position.x, uiAbove.position.y, uiAbove.position.z);
        faderLogic = this.gameObject.transform.GetChild(2).gameObject.GetComponent<FaderLogic>();
        firstFov = this.gameObject.transform.GetChild(3).gameObject;
        secondFov = this.gameObject.transform.GetChild(4).gameObject;
        timeToStare = 0.8f;
        stopHere = 3f;
        currentAngle1 = maxAngle;
        maxRadius3 = 4f;
        agent.acceleration = 700f;
        walkSpeed = 3;
        runSpeed = 9;
        randomIdle = Random.Range(0, 2);
        state = AIState.PATROLLING;
    }

    public void Update()
    {
        InFov();
        switch (state)
        {
            case AIState.PATROLLING: //gets called when player is out of any FOV
                if (investigatingState == 0)
                {
                    if (!goToNoisySource && !spottedHighlight)
                    {
                        if (!agent.pathPending && agent.remainingDistance < 0.5f)
                        {
                            GotoNextPoint(); // returns to its duties when player is not in sight or no noise was heard
                        }
                        if (patrolTurn)
                        {
                            if (destPoint == 2 || destPoint == stopDest)
                            {
                                StationeryRotation();
                            }
                        }
                    }
                    else if ((!goToNoisySource && spottedHighlight) || (goToNoisySource && spottedHighlight)) //investigates for player when out of suspicious view or/and heard from noisy source
                    {
                        if (isInFov != 2)
                        {
                            stopToLook += Time.deltaTime;
                            agent.SetDestination(playerHighlight.transform.position);
                            if (stopToLook <= timeToStare && !dontMove)
                            {
                                anim.SetInteger("State", randomIdle);
                                playerHighlight.transform.parent = null; //player leaves behind highlight for thug to investigate
                                agent.speed = 0;
                                targetDir = playerHighlight.transform.position - thisAI.position;
                                newDir = Vector3.RotateTowards(transform.forward, targetDir, 3f * Time.deltaTime, 0.0f);
                                transform.rotation = Quaternion.LookRotation(newDir);
                            }
                            else if (stopToLook >= timeToStare && !dontMove)
                            {
                                anim.SetInteger("State", 2);
                                agent.speed = walkSpeed;
                            }
            
                            CheckAndReturn(); //ensures after investigating, the thug returns to its duties
                        }
                        else if (isInFov == 2)  //invesitgates for player when out of danger FOV
                        {
                            anim.SetInteger("State", 2);
                            agent.speed = runSpeed;
                            stopHere = 3f;
                            playerHighlight.transform.parent = null;
                            agent.SetDestination(playerHighlight.transform.position);
                            CheckAndReturn(); //ensures after investigating, the thug returns to its duties
                        }
                    }
                    else if (goToNoisySource && !spottedHighlight) //ensures to investigate the location of the noise source
                    {
                        stopToLook += Time.deltaTime;
                        agent.SetDestination(noisySource.position);
                        if (stopToLook <= timeToStare)
                        {
                            anim.SetInteger("State", randomIdle);
                            agent.speed = 0;
                            targetDir = noisySource.transform.position - thisAI.position;
                            newDir = Vector3.RotateTowards(transform.forward, targetDir, 3f * Time.deltaTime, 0.0f);
                            transform.rotation = Quaternion.LookRotation(newDir);
                        }
                        else if (stopToLook >= timeToStare && !dontMove)
                        {
                            anim.SetInteger("State", 2);
                            agent.speed = walkSpeed;
                        }
                        CheckAndReturn(); //ensures after investigating, the thug returns to its duties
                    }
                }
                else if (investigatingState == 1)
                {
                    state = AIState.INVESTIGATING;
                }
                else if (investigatingState == 2)
                {
                    state = AIState.CHASE;
                }
                break;
            case AIState.INVESTIGATING:
                if (investigatingState == 0)
                {
                    state = AIState.PATROLLING;                
                }
                if (investigatingState == 1) //stares and approaches player if within the suspicious FOV
                {
                    spottedHighlight = true;
                    goToNoisySource = false;
                    playerHighlight.SetActive(false);
                    playerHighlight.transform.parent = playerTarget;
                    playerHighlight.transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
                    bgmLogic.ChangeDanger();
                    if (isInFov == 1)
                    {
                        playerHighlight.SetActive(false);
                        playerHighlight.transform.parent = null;
                        agent.SetDestination(playerHighlight.transform.position);
                        stopToLook += Time.deltaTime;
                        if (stopToLook <= timeToStare)
                        {
                            anim.SetInteger("State", randomIdle);
                            playerHighlight.transform.parent = null;
                            agent.speed = 0;
                            targetDir = playerHighlight.transform.position - thisAI.position;
                            newDir = Vector3.RotateTowards(transform.forward, targetDir, 3f * Time.deltaTime, 0.0f);
                            transform.rotation = Quaternion.LookRotation(newDir);
                        }
                        else if (stopToLook >= timeToStare)
                        {
                            anim.SetInteger("State", 2);
                            agent.speed = walkSpeed;

                        }
                    }
                }
                else if (investigatingState == 2)
                {
                    state = AIState.CHASE;
                }
                break;
            case AIState.CHASE:
                if (investigatingState == 0)
                {
                    spottedHighlight = true;
                    goToNoisySource = false;
                    agent.speed = runSpeed;
                    state = AIState.PATROLLING;
                    bgmLogic.EscapeDanger();
                }
                else if (investigatingState == 1)
                {
                    agent.speed = runSpeed;
                }
                else if (investigatingState == 2) //chases player if player is within danger FOV
                {
                    if (faderLogic.touchPlayer == false)
                    {
                        agent.speed = runSpeed;
                        anim.SetInteger("State", 2);
                    }
                    else if (faderLogic.touchPlayer == true)
                    {
                        agent.speed = 0;
                        anim.SetInteger("State", randomIdle);
                    }
                    agent.SetDestination(playerTarget.position);
                    playerHighlight.SetActive(false);
                    playerHighlight.transform.parent = playerTarget;
                    playerHighlight.transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
                    bgmLogic.ChangeDanger();
                }
                break;
        }
    }

    void GotoNextPoint() //ensures the thug resumes its guarding duties
    {
        firstFov.SetActive(true);
        secondFov.SetActive(false);
        questionMark.SetActive(false);
        exclamationMark.SetActive(false);
        if (!stationery && !staticRotate) //patrolling
        {
            if (aiPath.path_objs.Count == 0)
                return;
            agent.destination = aiPath.path_objs[destPoint].position;
            destPoint = (destPoint + 1) % aiPath.path_objs.Count;
            if (!patrolTurn)
            {
                anim.SetInteger("State", 2);
            }
            else if (patrolTurn)
            {
                if(destPoint != 2 && destPoint != stopDest)
                {
                    agent.speed = walkSpeed;
                    anim.SetInteger("State", 2);
                }
                else if(destPoint == 2 || destPoint == stopDest)
                {
                    agent.speed = 0;
                    anim.SetInteger("State", randomIdle);
                    timesHitRotation = 0;
                    cannotTurn = true;
                }
            }
        }
        else if (stationery && Vector3.Distance(thisAI.position, stationeryPosition.position) <= 1f) //static guarding
        {
            anim.SetInteger("State", randomIdle);
            if (!staticRotate)
            {
                rotatingSpeed = 0.5f;
                var desiredRotQ = Quaternion.Euler(new Vector3(lookHereStart.x, lookHereStart.y, lookHereStart.z));
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime * rotatingSpeed);
            }
            else if (staticRotate) //calls function for rotating properties
            {
                StationeryRotation();
            }
        }
        else if (stationery && Vector3.Distance(thisAI.position, stationeryPosition.position) >= 1f) //ensures the thug returns to its original position 
        {
            anim.SetInteger("State", 2);
            cannotTurn = true;
            agent.SetDestination(stationeryPosition.position);
        }
    }

    IEnumerator StopAndGo()
    {
        agent.speed = 0;
        yield return new WaitForSeconds(3f);
        agent.speed = walkSpeed;
    }


    void StationeryRotation() //function reserved for static rotation thugs
    {
        var up = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        Debug.DrawRay(transform.position, up * 10f, Color.black);

        if (!cannotTurn) //this raycast determines which way will the thug be rotating
        {
            rotatingSpeed = 30f;
            if (Physics.Raycast(transform.position, up, out hit, 10f, layerMask))
            {
                if (hit.transform.name == "RotatingLoop" && !turnBack)
                {
                    timeToResetView = 0.5f;
                    turnBack = true;
                }
                else if (hit.transform.name == "RotatingLoop" && turnBack)
                {
                    turnBack = false;
                }
                if(patrolTurn && hit.transform.name == "RotatingLoop")
                {
                    timesHitRotation += 1;
                }
            }
        }
        else if (cannotTurn) //ensure that the thug faces its starting angle before rotating left and right again after chasing player or investigating a noise source
        {
            print("face forward");
            rotatingSpeed = 0.5f;
            Quaternion desiredRotQ = Quaternion.Euler(new Vector3(lookHereStart.x, lookHereStart.y, lookHereStart.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime * rotatingSpeed);
            startToTurn += Time.deltaTime;
            if (startToTurn >= timeToResetView)
            {
                cannotTurn = false;
                startToTurn = 0;
            }
        }

        if (!turnBack)
        {
            transform.Rotate(0, Time.deltaTime * rotatingSpeed, 0);
        }
        else if (turnBack)
        {
            transform.Rotate(0, Time.deltaTime * -rotatingSpeed, 0);
        }
        if (patrolTurn) //ensures that any thug with only 2 points to patrol always rotates at each end
        {
            if(timesHitRotation == 2)
            {
                //stationeryPosition.position = aiPath.path_objs[1].position;
                //lookHereStart = new Vector3(-lookHereStart.x, -lookHereStart.y, -lookHereStart.z);
                agent.speed = walkSpeed;
                anim.SetInteger("State", 2);
                timesHitRotation = 0;
            }
        }
    }

    void CheckAndReturn() //causes thug to stop infront of highlight/noise source for a while and return to its duties
    {
        if ((!goToNoisySource && spottedHighlight) || (goToNoisySource && spottedHighlight))
        {
            target = playerHighlight.transform;
        }
        else if (goToNoisySource && !spottedHighlight)
        {
            target = noisySource;
            if (noisySource.name == "Shards")
            {
                noisySource.tag = "Untagged";
            }
            stopHere = 2f;
        }
        //if (Vector3.Distance(thisAI.position, target.position) < 8 && isInFov == 2)
        //{
        //playerHighlight.SetActive(true);
        //playerHighlight.transform.parent = null;
        //}
        if (Vector3.Distance(thisAI.position, target.position) < stopHere)
        {
            agent.speed = 0f;
            stopToGoBack += Time.deltaTime;
            if (stopToGoBack <= 1.5f)
            {
                dontMove = true;
                anim.SetInteger("State", randomIdle);
                if ((!goToNoisySource && spottedHighlight) || (goToNoisySource && spottedHighlight))
                {
                    playerHighlight.SetActive(true);
                    targetDir = playerHighlight.transform.position - thisAI.position;
                    newDir = Vector3.RotateTowards(transform.forward, targetDir, 3f * Time.deltaTime, 0.0f);
                    transform.rotation = Quaternion.LookRotation(newDir);
                }
            }
            else if (stopToGoBack >= 1.5f)  //resets the values
            {
                playerHighlight.SetActive(false);
                spottedHighlight = false;
                goToNoisySource = false;
                dontMove = false;
                noisySource = null;
                if (patrolTurn)
                {
                    destPoint += 1;
                }
                stopToGoBack = 0;
                stopToLook = 0;
                isInFov = 0;
                randomStop = 0;
                agent.speed = walkSpeed;
                maxAngle = currentAngle1;
                maxAngle2 = currentAngle1;
                stopHere = 3f;
                timeToResetView = 3f;
                bgmLogic.EscapeDanger();
                GotoNextPoint();
            }
        }
    }

    public bool InFov()
    {
        questionMark.transform.LookAt(Camera.main.transform);
        exclamationMark.transform.LookAt(Camera.main.transform); //ensures the exclamation and question mark icon is always looking at the main camera

        directionBetween = (playerTarget.position - thisAI.position).normalized;
        directionBetween.y *= 0; //height difference is able to influence its angle, it makes height is not a factor
        angle = Vector3.Angle(thisAI.forward, directionBetween); //ensures chasing only resumes when it is within the AI's view

        if (angle <= maxAngle) //when player is within this fov, thug slowly approaches
        {
            Ray ray = new Ray(thisAI.position, playerTarget.position - thisAI.position); //ensures the raycast is resting from this AI
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxRadius))
            {
                if (hit.transform == playerTarget && isInFov != 1 && isInFov !=2)
                {
                    stopToGoBack = 0;
                    investigatingState = 1;
                    isInFov = 1;
                    questionMark.SetActive(true);
                    exclamationMark.SetActive(false);
                    playerHighlight.transform.parent = playerTarget;
                    playerHighlight.transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
                    stopHere = 3f;
                    fileName = "ThugSuspicious";
                    SoundFX();
                }
            }
            else
            {
                investigatingState = 0;
            }
        }
        else
        {
            investigatingState = 0;
        }

        if (angle <= maxAngle3 && playerLogic.movingStyle == 1)  //detects players when running behind thugs
        {
            Ray ray = new Ray(thisAI.position, playerTarget.position - thisAI.position);
            RaycastHit hit2;
            if (Physics.Raycast(ray, out hit2, maxRadius3))
            {
                if (hit2.transform == playerTarget)
                {
                    fileName = "ThugAlert";
                    SoundFX();
                    AlertProperties(); //calls for the values and conditions to turn the thug into alert mode to chase the player
                }
            }
        }

        if (angle <= maxAngle2) //when player is within this fov, thug chases
        {
            Ray ray = new Ray(thisAI.position, playerTarget.position - thisAI.position);
            RaycastHit hit2;
            if (Physics.Raycast(ray, out hit2, maxRadius2) && isInFov != 2)
            {
                if (hit2.transform == playerTarget)
                {
                    fileName = "ThugAlert";
                    SoundFX();
                    AlertProperties(); //calls for the values and conditions to turn the thug into alert mode to chase the player
                    return true;
                }
            }
        }
        return false;
    }

    public void AlertProperties()
    {
        stopHere = 0;
        investigatingState = 2;
        isInFov = 2;
        agent.speed = runSpeed;
        agent.SetDestination(playerTarget.position);
        questionMark.SetActive(false);
        exclamationMark.SetActive(true);
        maxAngle = currentAngle1;
        maxAngle2 = currentAngle1;
        firstFov.SetActive(false);
        secondFov.SetActive(true);
        stopToGoBack = 0;
    }

    public void SoundFX() //call this function whenever you need a thug and bg sfx to be player, always state the fileName before calling the function
    {
        foreach(AudioSource thugAudio in externalAudio)
        {
            if (!thugAudio.isPlaying)
            {
                thugAudio.volume = 1;
                externalAudio[0].PlayOneShot((AudioClip)Resources.Load(fileName), 1f); //bg sound fx 
                externalAudio[1].PlayOneShot((AudioClip)Resources.Load(fileName2), 1f); //thug sound fx 
            }
        }
    }

    /*private void OnDrawGizmos() //the max angle determines how wide its fov will be based on the blue lines and the max radius determines how far will the fov be based on the yellow sphere
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxRadius2);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxRadius3);

        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * maxRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * maxRadius; //Ensures the second blue fov line goes the other angle
        Vector3 fov2Line1 = Quaternion.AngleAxis(maxAngle2, transform.up) * transform.forward * maxRadius2;
        Vector3 fov2Line2 = Quaternion.AngleAxis(-maxAngle2, transform.up) * transform.forward * maxRadius2;
        Vector3 fov3Line1 = Quaternion.AngleAxis(maxAngle3, transform.up) * transform.forward * maxRadius3;
        Vector3 fov3Line2 = Quaternion.AngleAxis(-maxAngle3, transform.up) * transform.forward * maxRadius3;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, fov2Line1);
        Gizmos.DrawRay(transform.position, fov2Line2);
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, fov3Line1);
        Gizmos.DrawRay(transform.position, fov3Line2);

        if (investigatingState == 0)
            Gizmos.color = Color.red;
        else if (investigatingState == 2)
            Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, (playerTarget.position - transform.position).normalized * maxRadius); //ensures the middle raycasting line turns to green when hitting the target
    }*/

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "NoisyFloor")
        {
            if (playerLogic.stepOnNoisyFloor == true && playerWithinRadius == true)
            {
                if (spottedHighlight == false) //prevents thugs from reacting to noisy floors when spotted YY
                {
                    fileName = "ThugSuspicious";
                    SoundFX();
                    noisySource = playerLogic.thisNoisyFloor;
                    goToNoisySource = true;
                    questionMark.SetActive(true);
                    exclamationMark.SetActive(false);
                }
            }
        }

        if (other.tag == "Player")
        {
            playerWithinRadius = true;
        }

        if (other.tag == "Bottle")
        {
            if(spottedHighlight == false) //prevents thug from reacting to bottles if he has spotted YY
            {
                noisySource = GameObject.Find("Shards").transform;
                goToNoisySource = true;
                questionMark.SetActive(true);
            }
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Thug" && isInFov == 2)
        {
            if (other.GetComponent<ArtificialIntelligence>().isInFov != 2)
            {
                other.GetComponent<ArtificialIntelligence>().spottedHighlight = true;
                other.GetComponent<ArtificialIntelligence>().investigatingState = 2;
                other.GetComponent<ArtificialIntelligence>().isInFov = 2;
                other.GetComponent<ArtificialIntelligence>().exclamationMark.SetActive(true);
                other.GetComponent<ArtificialIntelligence>().maxAngle = 45;
                other.GetComponent<ArtificialIntelligence>().maxAngle2 = 45;
                other.GetComponent<ArtificialIntelligence>().firstFov.SetActive(false);
                other.GetComponent<ArtificialIntelligence>().secondFov.SetActive(true);
            }
        }
    }*/

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerWithinRadius = false;
        }
    }

}