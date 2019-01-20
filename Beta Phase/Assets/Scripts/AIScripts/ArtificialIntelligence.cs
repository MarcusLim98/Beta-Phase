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
    public GameObject playerHighlight;
    public Transform noisySource;
    public Transform stationeryPosition;
    public GameObject suspicious, alert;
    public LayerMask layerMask;
    [Space]
    [Space]
    public AIPath aiPath;
    public float maxRadius, maxRadius2, maxAngle, maxAngle2, rotatingSpeed, walkSpeed, runSpeed, timeToStare;
    public bool spottedHighlight, goToNoisySource, stationery, staticRotate;
    [Space]
    [Space]
    [HideInInspector]
    public GameObject EmptyObj, questionMark, exclamationMark;
    [HideInInspector]
    public Vector3 lookHereStart;
    [HideInInspector]
    public int isInFov, investigatingState;
    NavMeshAgent agent;
    Animator anim;
    AudioSource externalAudio;
    Transform target, thisAI, uiAbove;
    Vector3 targetDir, newDir, directionBetween;
    Image uiState;
    PlayerLogic playerLogic;
    int destPoint = 0;
    float stopToLook, stopToGoBack, angle, startToTurn;
    bool turnBack, cannotTurn, playerWithinRadius;
    string fileName;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        thisAI = GetComponent<Transform>();
        externalAudio = GetComponent<AudioSource>();
        playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();
        if (stationery)
        {
            EmptyObj = new GameObject("Look Here");
            EmptyObj.transform.parent = this.gameObject.transform;
            EmptyObj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            stationeryPosition = this.gameObject.transform.GetChild(6);
            EmptyObj.transform.parent = null;
            lookHereStart = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        uiAbove = this.gameObject.transform.GetChild(5);
        questionMark = Instantiate(suspicious, transform.position, Quaternion.identity);
        exclamationMark = Instantiate(alert, transform.position, Quaternion.identity);
        questionMark.transform.parent = uiAbove;
        exclamationMark.transform.parent = uiAbove;
        questionMark.transform.position = new Vector3(uiAbove.position.x, uiAbove.position.y, uiAbove.position.z);
        exclamationMark.transform.position = new Vector3(uiAbove.position.x, uiAbove.position.y, uiAbove.position.z);
        timeToStare = 1.5f;
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
                            if (stopToLook <= timeToStare)
                            {
                                anim.SetInteger("State", 0);
                                playerHighlight.transform.parent = null;
                                agent.speed = 0;
                                targetDir = playerHighlight.transform.position - thisAI.position;
                                newDir = Vector3.RotateTowards(transform.forward, targetDir, 1.85f * Time.deltaTime, 0.0f);
                                transform.rotation = Quaternion.LookRotation(newDir);
                            }
                            else if (stopToLook >= timeToStare)
                            {
                                anim.SetInteger("State", 1);
                                agent.speed = walkSpeed;
                            }
                            CheckAndReturn();
                        }
                        else if (isInFov == 2)
                        {
                            anim.SetInteger("State", 1);
                            agent.speed = runSpeed;
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
                            anim.SetInteger("State", 0);
                            agent.speed = 0;
                            targetDir = noisySource.transform.position - thisAI.position;
                            newDir = Vector3.RotateTowards(transform.forward, targetDir, 1.85f * Time.deltaTime, 0.0f);
                            transform.rotation = Quaternion.LookRotation(newDir);
                        }
                        else if (stopToLook >= timeToStare)
                        {
                            anim.SetInteger("State", 1);
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
                    if (spottedHighlight)
                    {
                        playerHighlight.transform.parent = null;
                    }
                    state = AIState.PATROLLING;
                }
                if (investigatingState == 1)
                {
                    spottedHighlight = true;
                    goToNoisySource = false;
                    playerHighlight.SetActive(false);
                    playerHighlight.transform.parent = playerTarget;
                    playerHighlight.transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
                    if (isInFov == 1)
                    {
                        playerHighlight.SetActive(false);
                        playerHighlight.transform.parent = null;
                        agent.SetDestination(playerHighlight.transform.position);
                        stopToLook += Time.deltaTime;
                        if (stopToLook <= timeToStare)
                        {
                            anim.SetInteger("State", 0);
                            playerHighlight.transform.parent = null;
                            agent.speed = 0;
                            targetDir = playerHighlight.transform.position - thisAI.position;
                            newDir = Vector3.RotateTowards(transform.forward, targetDir, 1.85f * Time.deltaTime, 0.0f);
                            transform.rotation = Quaternion.LookRotation(newDir);
                        }
                        else if (stopToLook >= timeToStare)
                        {
                            anim.SetInteger("State", 1);
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
                    playerHighlight.transform.parent = null;
                    spottedHighlight = true;
                    goToNoisySource = false;
                    agent.speed = runSpeed;
                    state = AIState.PATROLLING;
                }
                else if (investigatingState == 1)
                {
                    agent.speed = runSpeed;
                }
                else if (investigatingState == 2)
                {
                    agent.speed = runSpeed;
                    anim.SetInteger("State", 1);
                    agent.SetDestination(playerTarget.position);
                    playerHighlight.SetActive(false);
                    playerHighlight.transform.parent = playerTarget;
                    playerHighlight.transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
                }
                break;
        }
    }

    void GotoNextPoint()
    {
        if (!stationery && !staticRotate)
        {
            anim.SetInteger("State", 1);
            if (aiPath.path_objs.Count == 0)
                return;
            agent.destination = aiPath.path_objs[destPoint].position;
            destPoint = (destPoint + 1) % aiPath.path_objs.Count;
        }
        else if (stationery && Vector3.Distance(thisAI.position, stationeryPosition.position) <= 1f)
        {
            questionMark.SetActive(false);
            exclamationMark.SetActive(false);
            anim.SetInteger("State", 0);
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
            anim.SetInteger("State", 1);
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
                    turnBack = true;
                }
                else if (hit.transform.name == "RotatingLoop" && turnBack)
                {
                    turnBack = false;
                }
            }
        }
        else if (cannotTurn)
        {
            rotatingSpeed = 0.5f;
            var desiredRotQ = Quaternion.Euler(new Vector3(lookHereStart.x, lookHereStart.y, lookHereStart.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime * rotatingSpeed);
            startToTurn += Time.deltaTime;
            if (startToTurn >= 3f)
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
        }
        if (Vector3.Distance(thisAI.position, target.position) < 6)
        {
            playerHighlight.SetActive(true);
            playerHighlight.transform.parent = null;
        }
        if (Vector3.Distance(thisAI.position, target.position) < 3)
        {
            stopToGoBack += Time.deltaTime;
            if (stopToGoBack <= 3f)
            {
                anim.SetInteger("State", 0);
                agent.speed = 0f;
            }
            else if (stopToGoBack >= 3f)
            {
                questionMark.SetActive(false);
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
                if (hit.transform == playerTarget && isInFov != 2)
                {
                    print("spotted");
                    investigatingState = 1;
                    isInFov = 1;
                    questionMark.SetActive(true);
                    exclamationMark.SetActive(false);
                    playerHighlight.transform.parent = playerTarget;
                    playerHighlight.transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
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

        if (angle <= maxAngle2)
        {
            Ray ray = new Ray(thisAI.position, playerTarget.position - thisAI.position);
            RaycastHit hit2;
            if (Physics.Raycast(ray, out hit2, maxRadius2))
            {
                if (hit2.transform == playerTarget)
                {
                    investigatingState = 2;
                    isInFov = 2;
                    questionMark.SetActive(false);
                    exclamationMark.SetActive(true);
                    fileName = "ThugAlert";
                    SoundFX();
                    return true;
                }
            }
        }
        return false;
    }

    void SoundFX()
    {
        if (!externalAudio.isPlaying)
        {
            externalAudio.PlayOneShot((AudioClip)Resources.Load(fileName), 1f);
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

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "NoisyFloor")
        {
            if (playerLogic.stepOnNoisyFloor == true && playerWithinRadius == true)
            {
                noisySource = playerLogic.thisNoisyFloor;
                goToNoisySource = true;
                questionMark.SetActive(true);
                exclamationMark.SetActive(false);
            }
        }

        if (other.tag == "Player")
        {
            playerWithinRadius = true;
        }

        if (other.tag == "Bottle")
        {
            noisySource = GameObject.Find("Shards").transform;
            goToNoisySource = true;
            questionMark.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Thug" && isInFov == 2)
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

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerWithinRadius = false;
        }
    }

}