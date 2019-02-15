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
    public Transform noisySource;
    public GameObject alert, gunLine, bullet, muzzleFlash, fadeToBlack;
    public AIVision aiVision;
    //public Text toBeContinued;
    [Space]
    [Space]
    public AIPath[] aiPath;
    public ArtificialIntelligence aiFollower;
    public float maxRadius, maxAngle, maxRadius2, maxAngle2, rotatingSpeed, walkSpeed, runSpeed;
    public int investigatingState,pathWay;
    public bool spottedHighlight, goToNoisySource;
    public bool stopLaoDa;//stops lao da from moving when he reaches the end point of his paths, it triggers automatically
    public bool triggerFirstEvent; //controls the fov whether it can react to YY
    [Space]
    [Space]
    NavMeshAgent agent;
    Animator anim;
    GameObject exclamationMark;
    Transform target, thisAI, uiAbove;
    Vector3 targetDir, newDir, directionBetween, lookHereStart;
    Image uiState;
    PlayerLogic playerLogic;
    AudioSource externalAudio;
    public int timesFired, timesHit;
    int destPoint = 0, isInFov, firstStage, canFire, hitByCrate;
    float stopToGoBack, angle, stopToLook;
    bool turnBack, cannotTurn, playerWithinRadius;
    string fileName;
    [Space]
    [Space]
    public ArtificialIntelligence[] thugsToCall;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        thisAI = GetComponent<Transform>();
        externalAudio = GetComponent<AudioSource>();
        playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();

        uiAbove = this.gameObject.transform.GetChild(4);
        exclamationMark = Instantiate(alert, transform.position, Quaternion.identity);
        exclamationMark.transform.parent = uiAbove;
        exclamationMark.transform.position = new Vector3(uiAbove.position.x, uiAbove.position.y, uiAbove.position.z);
        lookHereStart = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        walkSpeed = 3;
        agent.speed = 0;
        anim.SetInteger("State", 0);
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
                    if (!stopLaoDa && triggerFirstEvent)
                    {
                        anim.SetInteger("State", 1);
                        agent.speed = walkSpeed;
                    }
                    else if (stopLaoDa /*&& !triggerFirstEvent*/)                   //don't move, first cutscene not done
                    {
                        anim.SetInteger("State", 0);
                        agent.speed = 0;
                        destPoint = 0;
                    }
                    if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    {
                        GotoNextPoint();
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
                    agent.speed = walkSpeed;
                    exclamationMark.SetActive(false);
                    gunLine.SetActive(false);
                    aiVision.angle = 51;
                    stopToLook = 0;
                    if (canFire == 1)
                    {
                        playerHighlight.SetActive(true);
                        playerHighlight.transform.parent = null;
                        aiFollower.playerMask = LayerMask.GetMask("Nothing");
                        aiFollower.maxRadius3 = 2.5f;
                        aiFollower.stopHere = 3;
                    }
                    canFire = 0;
                    state = AIState.PATROLLING;
                }
                else if (investigatingState == 1)
                {
                    FiringPropeties();
                }
                break;
            case AIState.CHASE:
                break;
        }
    }

    void FiringPropeties()
    {
        //triggerFirstEvent = true;
        anim.SetInteger("State", 2);
        agent.speed = 0;
        exclamationMark.SetActive(true);
        gunLine.SetActive(true);
        playerHighlight.SetActive(false);
        playerHighlight.transform.parent = playerTarget;
        playerHighlight.transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
        walkSpeed = 6;
        if (aiVision.angle >= 4)
        {
            print("fire");
            aiVision.angle -= 1;
            muzzleFlash.SetActive(false);
            if (aiVision.angle >= 23)
            {
                targetDir = playerHighlight.transform.position - thisAI.position;
                newDir = Vector3.RotateTowards(transform.forward, targetDir, rotatingSpeed * Time.deltaTime, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDir);
            }
        }
        else if (aiVision.angle <= 4)
        {
            if(canFire == 0)
            {
                fileName = "LaoDaGunShot";
                SoundFX();
                muzzleFlash.SetActive(true);
                Instantiate(bullet, transform.position, Quaternion.Euler(90, 0, 0));
                aiFollower.playerMask = LayerMask.GetMask("Player");
                aiFollower.maxRadius3 = 60;
                aiFollower.stopHere = 0;
                canFire = 1;
                rotatingSpeed = 1.85f;
            }
        }
    }

    void GotoNextPoint()
    {
        if(destPoint + 1 == aiPath[pathWay].path_objs.Count)
        {
            if (pathWay != 4)
            {
                stopLaoDa = true;
                pathWay += 1;
            }
        }
        if (aiPath[pathWay].path_objs.Count == 0)
            return;
        agent.destination = aiPath[pathWay].path_objs[destPoint].position;
        destPoint = (destPoint + 1) % aiPath[pathWay].path_objs.Count;
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
                if (hit.transform == playerTarget && triggerFirstEvent)
                {
                    investigatingState = 1;
                    fileName = "ThugAlert";
                    SoundFX();
                    rotatingSpeed = 1.85f;
                    /*investigatingState = 1;
                    goToNoisySource = false;
                    stopToLook = 0;
                    exclamationMark.SetActive(true);
     
                    playerHighlight.SetActive(false);
                    playerHighlight.transform.parent = playerTarget;
                    playerHighlight.transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
                    if (firstStage < 3)
                    {
                        foreach (ArtificialIntelligence thugs in thugsToCall)
                        {
                            thugs.spottedHighlight = true;
                            thugs.investigatingState = 2;
                            thugs.isInFov = 2;
                            thugs.exclamationMark.SetActive(true);
                        }
                    }*/
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

        if (angle <= maxAngle2 && playerLogic.movingStyle == 1)
        {
            Ray ray = new Ray(thisAI.position, playerTarget.position - thisAI.position);
            RaycastHit hit2;
            if (Physics.Raycast(ray, out hit2, maxRadius2) && isInFov != 2)
            {
                if (hit2.transform == playerTarget && triggerFirstEvent)
                {
                    investigatingState = 1;
                    fileName = "ThugAlert";
                    SoundFX();
                    rotatingSpeed = 10f;
                }
            }
        }

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
        Gizmos.color = Color.cyan;
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
        /*if (other.tag == "Player")
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

        if (other.tag == "Thug" && timesFired >= 6 && spottedHighlight)
        {
            if (other.GetComponent<ArtificialIntelligence>().isInFov != 2)
            {
                other.GetComponent<ArtificialIntelligence>().spottedHighlight = true;
                other.GetComponent<ArtificialIntelligence>().investigatingState = 2;
                other.GetComponent<ArtificialIntelligence>().isInFov = 2;
                other.GetComponent<ArtificialIntelligence>().exclamationMark.SetActive(true);
            }
        }*/
    }

    private void OnTriggerStay(Collider other)
    {
        /*if (other.name == "NoisyFloor" && timesFired >= 6)
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
        }*/
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
        //toBeContinued.enabled = true;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("FakeMenu");
    }
}
