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
    public LayerMask layerMask, playerMask;
    [Space]
    [Space]
    public AIPath aiPath;
    public float maxRadius, maxRadius2, maxRadius3,maxRadius4, maxAngle, maxAngle2, maxAngle3, maxAngle4, rotatingSpeed, walkSpeed, runSpeed, timeToStare, stopToGoBack, stopHere;
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
    AudioSource externalAudio;
    Transform target, thisAI, uiAbove;
    Vector3 targetDir, newDir, directionBetween;
    Image uiState;
    PlayerLogic playerLogic;
    FaderLogic faderLogic;
    BGMControl bgmLogic;
    int destPoint = 0, timesHitRotation, randomIdle;
    float stopToLook, angle, startToTurn, timeToResetView, currentAngle1;
    bool turnBack, cannotTurn, playerWithinRadius, dontMove;
    public string fileName;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        thisAI = GetComponent<Transform>();
        externalAudio = GetComponent<AudioSource>();
        playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();
        bgmLogic = GameObject.Find("BGMS").GetComponent<BGMControl>();
        if (stationery)
        {
            EmptyObj = new GameObject("Look Here");
            EmptyObj.transform.parent = this.gameObject.transform;
            stationeryPosition = this.gameObject.transform.GetChild(6);
            if (!followingLaoDa)
            {
                EmptyObj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                EmptyObj.transform.parent = null;
            }
            else if (followingLaoDa)
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
            case AIState.PATROLLING:
                if (investigatingState == 0)
                {
                    if (!goToNoisySource && !spottedHighlight)
                    {
                        if (!agent.pathPending && agent.remainingDistance < 0.5f)
                        {
                            GotoNextPoint();
                        }
                    }
                    else if ((!goToNoisySource && spottedHighlight) || (goToNoisySource && spottedHighlight))
                    {
                        if (isInFov != 2)
                        {
                            stopToLook += Time.deltaTime;
                            agent.SetDestination(playerHighlight.transform.position);
                            if (stopToLook <= timeToStare && !dontMove)
                            {
                                anim.SetInteger("State", randomIdle);
                                playerHighlight.transform.parent = null;
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
            
                            CheckAndReturn();
                        }
                        else if (isInFov == 2)
                        {
                            anim.SetInteger("State", 2);
                            agent.speed = runSpeed;
                            stopHere = 3f;
                            playerHighlight.transform.parent = null;
                            agent.SetDestination(playerHighlight.transform.position);
                            CheckAndReturn();
                        }
                    }
                    else if (goToNoisySource && !spottedHighlight)
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
                        CheckAndReturn();
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
                if (investigatingState == 1)
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
                else if (investigatingState == 2)
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

    void GotoNextPoint()
    {
        firstFov.SetActive(true);
        secondFov.SetActive(false);
        questionMark.SetActive(false);
        exclamationMark.SetActive(false);
        if (!stationery && !staticRotate)
        {
            anim.SetInteger("State", 2);
            if (aiPath.path_objs.Count == 0)
                return;
            agent.destination = aiPath.path_objs[destPoint].position;
            destPoint = (destPoint + 1) % aiPath.path_objs.Count;
        }
        else if (stationery && Vector3.Distance(thisAI.position, stationeryPosition.position) <= 1f)
        {
            anim.SetInteger("State", randomIdle);
            if (!staticRotate)
            {
                rotatingSpeed = 0.5f;
                var desiredRotQ = Quaternion.Euler(new Vector3(lookHereStart.x, lookHereStart.y, lookHereStart.z));
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime * rotatingSpeed);
            }
            else if (staticRotate)
            {
                StationeryRotation();
            }
        }
        else if (stationery && Vector3.Distance(thisAI.position, stationeryPosition.position) >= 1f)
        {
            anim.SetInteger("State", 2);
            cannotTurn = true;
            agent.SetDestination(stationeryPosition.position);
        }
    }

    void StationeryRotation()
    {
        var up = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        Debug.DrawRay(transform.position, up * 10f, Color.black);

        if (!cannotTurn)
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
        else if (cannotTurn)
        {
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
        if (patrolTurn)
        {
            if(timesHitRotation == 2)
            {
                stationeryPosition.position = aiPath.path_objs[1].position;
                lookHereStart = new Vector3(-lookHereStart.x, -lookHereStart.y, -lookHereStart.z);
                timesHitRotation = 3;
            }
            else if(timesHitRotation == 5)
            {
                stationeryPosition.position = aiPath.path_objs[0].position;
                lookHereStart = new Vector3(lookHereStart.x, lookHereStart.y + 180, lookHereStart.z);
                timesHitRotation = 0;
            }
        }
    }

    void CheckAndReturn()
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
                    if(investigatingState != 2)
                    {
                        playerHighlight.SetActive(true);
                    }
                    targetDir = playerHighlight.transform.position - thisAI.position;
                    newDir = Vector3.RotateTowards(transform.forward, targetDir, 3f * Time.deltaTime, 0.0f);
                    transform.rotation = Quaternion.LookRotation(newDir);
                }
            }
            else if (stopToGoBack >= 1.5f)
            {
                playerHighlight.SetActive(false);
                spottedHighlight = false;
                goToNoisySource = false;
                dontMove = false;
                noisySource = null;
                stopToGoBack = 0;
                stopToLook = 0;
                isInFov = 0;
                agent.speed = walkSpeed;
                maxAngle = 40;
                maxAngle2 = 40;
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
        exclamationMark.transform.LookAt(Camera.main.transform);

        directionBetween = (playerTarget.position - thisAI.position).normalized;
        directionBetween.y *= 0; //height difference is able to influence its angle, it makes height is not a factor
        angle = Vector3.Angle(thisAI.forward, directionBetween); //ensures chasing only resumes when it is within the AI's view

        if (angle <= maxAngle)
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
                    SuspiciousProperties();
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

        if (angle <= maxAngle3 && playerLogic.movingStyle == 1)
        {
            Ray ray = new Ray(thisAI.position, playerTarget.position - thisAI.position);
            RaycastHit hit2;
            if (Physics.Raycast(ray, out hit2, maxRadius3))
            {
                if (hit2.transform == playerTarget)
                {
                    fileName = "ThugAlert";
                    SoundFX();
                    AlertProperties();
                }
            }
        }

        if (angle <= maxAngle2)
        {
            Ray ray = new Ray(thisAI.position, playerTarget.position - thisAI.position);
            RaycastHit hit2;
            if (Physics.Raycast(ray, out hit2, maxRadius2) && isInFov != 2)
            {
                if (hit2.transform == playerTarget)
                {
                    fileName = "ThugAlert";
                    SoundFX();
                    AlertProperties();
                    return true;
                }
            }
        }
        return false;
    }

    void SuspiciousProperties()
    {
        questionMark.SetActive(true);
        exclamationMark.SetActive(false);
        playerHighlight.transform.parent = playerTarget;
        playerHighlight.transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
        stopHere = 3f;
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

    public void SoundFX()
    {
        if (!externalAudio.isPlaying)
        {
            externalAudio.volume = 1;
            externalAudio.PlayOneShot((AudioClip)Resources.Load(fileName), 1f);
        }
    }

    private void OnDrawGizmos() //the max angle determines how wide its fov will be based on the blue lines and the max radius determines how far will the fov be based on the yellow sphere
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
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "NoisyFloor")
        {
            if (playerLogic.stepOnNoisyFloor == true && playerWithinRadius == true)
            {
                if (spottedHighlight == false)
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
            if(spottedHighlight == false)
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