using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class AIBoss : MonoBehaviour {
    enum AIState { PATROLLING, INVESTIGATING, CHASE }; // States
    AIState state;

    public Transform playerTarget;
    public GameObject playerHighlight;
    public Transform noisySource, stationeryPosition;
    public GameObject alert, gunLine, bullet, crate, muzzleFlash, fadeToBlack;
    public AIVision aiVision;
    public Text toBeContinued;
    [Space]
    [Space]
    public AIPath aiPath;
    public float maxRadius, maxRadius2, maxAngle, maxAngle2, rotatingSpeed, walkSpeed, runSpeed;
    public bool spottedHighlight, goToNoisySource, stationery;
    [Space]
    [Space]
    NavMeshAgent agent;
    Animator anim;
    GameObject EmptyObj, exclamationMark;
    Transform target, thisAI, uiAbove;
    Vector3 targetDir, newDir, directionBetween, lookHereStart;
    Image uiState;
    PlayerLogic playerLogic;
    AudioSource externalAudio;
    public int timesFired, timesHit;
    int destPoint = 0, isInFov, firstStage, canFire, investigatingState, hitByCrate;
    float stopToLook, stopToGoBack, angle;
    bool turnBack, cannotTurn, playerWithinRadius;
    [Space]
    [Space]
    public ArtificialIntelligence[] thugsToCall;

    public void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();
        thisAI = GetComponent<Transform>();
        externalAudio = GetComponent<AudioSource>();
        externalAudio.clip = (AudioClip)Resources.Load("LaoDaGunShot");
        playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();
        if (stationery)
        {
            EmptyObj = new GameObject("Look Here");
            EmptyObj.transform.parent = this.gameObject.transform;
            EmptyObj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            stationeryPosition = this.gameObject.transform.GetChild(8);
            EmptyObj.transform.parent = null;
            lookHereStart = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        uiAbove = this.gameObject.transform.GetChild(7);
        exclamationMark = Instantiate(alert, transform.position, Quaternion.identity);
        exclamationMark.transform.parent = uiAbove;
        exclamationMark.transform.position = new Vector3(uiAbove.position.x, uiAbove.position.y, uiAbove.position.z);
        state = AIState.PATROLLING;
    }

    public void Update()
    {
        InFov();
        switch (state)
        {
            case AIState.PATROLLING:
                if (investigatingState == 0) //1st stage
                {
                    if (!goToNoisySource && firstStage < 3)
                    {
                        //print("1");
                        exclamationMark.SetActive(false);
                        rotatingSpeed = 1.5f;
                        var desiredRotQ = Quaternion.Euler(new Vector3(lookHereStart.x, lookHereStart.y, lookHereStart.z));
                        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime * rotatingSpeed);
                    }
                    else if (!goToNoisySource && !spottedHighlight && firstStage >= 3)
                    {
                        //print("2");
                        agent.speed = walkSpeed;
                        anim.SetInteger("State", 1);
                        if (timesFired <= 6)
                        {
                            exclamationMark.SetActive(false);
                            gunLine.SetActive(false);
                            aiVision.angle = 51;
                            canFire = 0;
                        }
                        if (!agent.pathPending && agent.remainingDistance < 0.5f)
                        {
                            GotoNextPoint();
                        }
                    }
                    else if (goToNoisySource && !spottedHighlight)
                    {
                        //print("3");
                        stopToLook += Time.deltaTime;
                        if (stopToLook <= 1.5f)
                        {
                            anim.SetInteger("State", 0);
                            agent.speed = 0;
                            targetDir = noisySource.transform.position - thisAI.position;
                            newDir = Vector3.RotateTowards(transform.forward, targetDir, 1.85f * Time.deltaTime, 0.0f);
                            transform.rotation = Quaternion.LookRotation(newDir);
                        }
                        else if (stopToLook >= 1.5f)
                        {
                            if (noisySource.name == "Shards")
                            {
                                noisySource.tag = "Untagged";
                            }
                            stopToLook = 0;
                            goToNoisySource = false;
                            exclamationMark.SetActive(false);
                        }
                    }
                    else if (!goToNoisySource && spottedHighlight)
                    {
                        //print("4");
                        agent.SetDestination(playerHighlight.transform.position);
                        playerHighlight.transform.parent = null;
                        CheckAndReturn();
                    }
                }
                else if (investigatingState == 1)
                {
                    state = AIState.INVESTIGATING;
                }
                break;
            case AIState.INVESTIGATING:
                if (investigatingState == 0)
                {
                    state = AIState.PATROLLING;
                }
                else if (investigatingState == 1)
                {
                    if (!goToNoisySource && timesFired < 6)
                    {
                        //print("5");
                        anim.SetInteger("State", 2);
                        agent.speed = 0;
                        playerHighlight.SetActive(false);
                        playerHighlight.transform.parent = playerTarget;
                        playerHighlight.transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
                        if (firstStage >= 3 && timesFired <= 6)
                        {
                            gunLine.SetActive(true);
                            if (aiVision.angle >= 4)
                            {
                                targetDir = playerHighlight.transform.position - thisAI.position;
                                newDir = Vector3.RotateTowards(transform.forward, targetDir, 1.85f * Time.deltaTime, 0.0f);
                                transform.rotation = Quaternion.LookRotation(newDir);
                                aiVision.angle -= 1;
                                muzzleFlash.SetActive(false);
                            }
                            else if (aiVision.angle <= 4)
                            {
                                if (canFire == 0)
                                {
                                    Instantiate(bullet, transform.position, Quaternion.Euler(90, 0, 0));
                                    externalAudio.Play();
                                    canFire = 1;
                                    timesFired += 1;
                                    muzzleFlash.SetActive(true);
                                    if (timesFired == 6)
                                    {
                                        stationery = false;
                                        gunLine.SetActive(false);
                                        foreach (ArtificialIntelligence thugs in thugsToCall)
                                        {
                                            thugs.runSpeed = 9f;
                                            thugs.walkSpeed = 9f;
                                            thugs.timeToStare = 0.1f;
                                            thugs.questionMark = thugs.exclamationMark;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (!goToNoisySource && timesFired >= 6)
                    {
                        //print("6");
                        agent.speed = runSpeed;
                        anim.SetInteger("State", 1);
                        spottedHighlight = true;
                        goToNoisySource = false;
                        agent.SetDestination(playerTarget.position);
                        playerHighlight.SetActive(false);
                        playerHighlight.transform.parent = playerTarget;
                        playerHighlight.transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
                    }
                }
                break;
            case AIState.CHASE:
                break;
        }
    }

    void GotoNextPoint()
    {
        print("patrolling");
        if (aiPath.path_objs.Count == 0)
            return;
        agent.destination = aiPath.path_objs[destPoint].position;
        destPoint = (destPoint + 1) % aiPath.path_objs.Count;
    }

    public bool InFov()
    {
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
                if (hit.transform == playerTarget)
                {
                    investigatingState = 1;
                    goToNoisySource = false;
                    stopToLook = 0;
                    exclamationMark.SetActive(true);
                    if (firstStage < 3)
                    {
                        foreach (ArtificialIntelligence thugs in thugsToCall)
                        {
                            thugs.spottedHighlight = true;
                            thugs.investigatingState = 2;
                            thugs.isInFov = 2;
                            thugs.exclamationMark.SetActive(true);
                        }
                    }
                    return true;
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(crate, new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z), Quaternion.Euler(0, 0, 0));
        }

        RaycastHit hit2;
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * 3, Color.red);
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up) * 3, out hit2, 3))
        {
            if (hit2.transform.tag == "Crate" && hitByCrate == 0)
            {
                hitByCrate = 1;
                agent.speed -= 0.5f;
                walkSpeed -= 0.5f;
                runSpeed -= 0.5f;
                timesHit += 1;
                if (timesHit == 4)
                {
                    fadeToBlack.SetActive(true);
                    StartCoroutine(EndGame());
                }
            }
        }
        else hitByCrate = 0;
        return false;
    }

    void CheckAndReturn()
    {
        if (Vector3.Distance(thisAI.position, playerHighlight.transform.position) < 6)
        {
            playerHighlight.SetActive(true);
        }
        if (Vector3.Distance(thisAI.position, playerHighlight.transform.position) < 3)
        {
            stopToGoBack += Time.deltaTime;
            if (stopToGoBack <= 3f)
            {
                anim.SetInteger("State", 0);
                agent.speed = 0f;
            }
            else if (stopToGoBack >= 3f)
            {
                exclamationMark.SetActive(false);
                playerHighlight.SetActive(false);
                spottedHighlight = false;
                goToNoisySource = false;
                stopToGoBack = 0;
                stopToLook = 0;
                isInFov = 0;
                agent.speed = walkSpeed;
                GotoNextPoint();
            }
        }
    }

    private void OnDrawGizmos() //the max angle determines how wide its fov will be based on the blue lines and the max radius determines how far will the fov be based on the yellow sphere
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius);
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, maxRadius2);

        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * maxRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * maxRadius; //Ensures the second blue fov line goes the other angle
        Vector3 fov2Line1 = Quaternion.AngleAxis(maxAngle2, transform.up) * transform.forward * maxRadius2;
        Vector3 fov2Line2 = Quaternion.AngleAxis(-maxAngle2, transform.up) * transform.forward * maxRadius2;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, fov2Line1);
        Gizmos.DrawRay(transform.position, fov2Line2);

        if (investigatingState == 0)
            Gizmos.color = Color.red;
        else if (investigatingState == 2)
            Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, (playerTarget.position - transform.position).normalized * maxRadius); //ensures the middle raycasting line turns to green when hitting the target
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerWithinRadius = true;
            if (firstStage < 3)
            {
                firstStage += 1;
            }
        }

        if (other.tag == "Bottle")
        {
            noisySource = GameObject.Find("Shards").transform;
            target = noisySource;
            goToNoisySource = true;
            exclamationMark.SetActive(true);
        }

        if (other.name == "Path" && timesFired >= 6 && investigatingState == 1)
        {
            aiPath = other.GetComponentInParent<AIPath>();
        }

        if (other.name == "Crate(Clone)")
        {
            if (firstStage < 3)
            {
                firstStage = 3;
                timesFired = 6;
            }
            if (timesFired == 6)
            {
                walkSpeed -= 0.5f;
                runSpeed -= 0.5f;
                agent.speed -= 0.5f;
            }
        }

        if (other.tag == "Thug" && timesFired >= 6 && spottedHighlight)
        {
            if (other.GetComponent<ArtificialIntelligence>().isInFov != 2)
            {
                other.GetComponent<ArtificialIntelligence>().spottedHighlight = true;
                other.GetComponent<ArtificialIntelligence>().investigatingState = 2;
                other.GetComponent<ArtificialIntelligence>().isInFov = 2;
                other.GetComponent<ArtificialIntelligence>().exclamationMark.SetActive(true);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "NoisyFloor" && timesFired >= 6)
        {
            if (playerLogic.stepOnNoisyFloor == true && playerWithinRadius == true)
            {
                noisySource = playerLogic.thisNoisyFloor;
                goToNoisySource = true;
                exclamationMark.SetActive(true);
                playerHighlight.transform.parent = playerTarget;
                playerHighlight.transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
                if (other.transform.GetChild(0).name == "NoisyFloor1")
                {
                    thugsToCall[0].spottedHighlight = true;
                    thugsToCall[0].investigatingState = 2;
                    thugsToCall[0].isInFov = 2;
                    thugsToCall[0].exclamationMark.SetActive(true);
                    thugsToCall[0].questionMark.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerWithinRadius = false;
        }
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2f);
        toBeContinued.enabled = true;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("FakeMenu");
    }
}
