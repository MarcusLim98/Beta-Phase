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
    public AIPath aiPath;
    public float maxRadius, maxAngle, rotatingSpeed, walkSpeed, runSpeed;
    public int investigatingState;
    public bool spottedHighlight, goToNoisySource, stationery;
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
    float stopToLook, stopToGoBack, angle;
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
        //state = AIState.PATROLLING;
    }

    public void Update()
    {
        InFov();
        print(state);
        switch (state)
        {
            case AIState.PATROLLING:
                if(investigatingState == 0)
                {
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
                    anim.SetInteger("State", 1);
                    agent.speed = walkSpeed;
                    state = AIState.PATROLLING;
                }
                else if (investigatingState == 1)
                {
                    agent.speed = 0;
                    anim.SetInteger("State", 3);
                    targetDir = playerHighlight.transform.position - thisAI.position;
                    newDir = Vector3.RotateTowards(transform.forward, targetDir, 1.85f * Time.deltaTime, 0.0f);
                    transform.rotation = Quaternion.LookRotation(newDir);
                }
                break;
            case AIState.CHASE:
                break;
        }
    }

    void GotoNextPoint()
    {
        anim.SetInteger("State", 1);
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
                    /*fileName = "ThugAlert";
                    SoundFX();
                    investigatingState = 1;
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

        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * maxRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * maxRadius; //Ensures the second blue fov line goes the other angle

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

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
