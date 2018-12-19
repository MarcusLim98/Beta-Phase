using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class ArtificialIntelligence : MonoBehaviour
{
    //Navmesh method
    public Transform playerTarget, noiseTarget; //ensures which game object it will be chasing
    public GameObject playerOutline, visualState, EM, QM, questionMark, exclamationMark;
    public int enemyType; //sets whether you want the ai to be patrolling or chasing (1 = patrolling, 2 = chases when player is within fov, 4 = only rotates but still chases player)
    public int specialType; //1 = throws bottle at player, 2 = investigates the spot where 1 has thrown while also finding yy, 3 = spotter
    public float gap; //a value for the agent to leave a gap between itself and the intended path it is travelling towards when changing its destination point
    public float maxAngle;
    public float maxRadius;
    public float Timer;
    public float sphereRadius;
    public float raycastLength;
    public int destPoint; //a value to keep track of which destination point
    [Space]
    [Space]
    public Vector3 lastSeen;
    public Vector3 staticOriginalRotation;
    public Vector3 raycastForSpotting;
    public Transform lookA, lookB;
    [Space]
    [Space]
    public Transform[] points; //an array to store all of the destination point
    public ArtificialIntelligence[] callForThugs;
    NavMeshAgent agent;
    private Transform thisAI;
	private Vector3 sphereCenter;
	private Vector3 currentPosition;
    public bool staticRotate;

    [HideInInspector]
    public Image qmImage, emImage;
    int originalType, spotterIntervals;
    float timer;
    [HideInInspector]
    public bool isInFov = false, onStay = false, isInCollider = false, hasShotBottle, run;
    [HideInInspector]
    public Animator anim;
    SpotCheck spotCheck;
    BottleThrow bottleThrow;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        thisAI = GetComponent<Transform>();
        originalType = enemyType;
        spotCheck = transform.GetComponentInChildren<SpotCheck>();
        anim = GetComponent<Animator>();
        bottleThrow = GameObject.Find("BottleToss").GetComponent<BottleThrow>();
        questionMark = Instantiate(QM, new Vector3(transform.position.x, transform.position.x, transform.position.z), Quaternion.identity);
        questionMark.transform.SetParent(GameObject.Find("CanvasAIVisuals").transform);
        exclamationMark = Instantiate(EM, new Vector3(transform.position.x, transform.position.x, transform.position.z), Quaternion.identity);
        exclamationMark.transform.SetParent(GameObject.Find("CanvasAIVisuals").transform);
        visualState = questionMark;
        qmImage = questionMark.GetComponent<Image>();
        emImage = exclamationMark.GetComponent<Image>();
        qmImage.enabled = false;
        emImage.enabled = false;
    }

    void Update()
    {
        if (isInFov)
        {
            Chase();
        }
        else if (!isInFov)
        {
            FOVProperties();

            if (enemyType == 3)
            {
                CheckNoise();
            }
            else if (enemyType == 1)
            {
                GotoNextPoint();
            }
        }

        /*if (spotCheck.atNoiseSource)
        {
            timer = timer + Time.deltaTime;
        }
        else timer = 0f;*/
        Vector3 statePos = Camera.main.WorldToScreenPoint(new Vector3(this.transform.position.x, this.transform.position.y + 3f, this.transform .position.z));
        visualState.transform.position = statePos;
    }

    void GotoNextPoint() //Those red cubes are its destination points but an empty game object works fine as well
    {
        agent.speed = 3;
        agent.angularSpeed = 200;
        agent.acceleration = 8;
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.SetDestination(points[destPoint].position);

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        if (!agent.pathPending && agent.remainingDistance < gap)
        {
            destPoint = (destPoint + 1) % points.Length;
        }

        if (!staticRotate && this.gameObject.name != "StaticAI")
        {
            anim.SetInteger("State", 1);
        }
        else if (!staticRotate && this.gameObject.name == "StaticAI")
        {
            if (agent.remainingDistance < 1f)
            {
                if (!run)
                {
                    anim.SetInteger("State", 0);
                    qmImage.enabled = false;
                    emImage.enabled = false;
                }
                else if (run)
                {
                    anim.SetInteger("State", 1);
                    qmImage.enabled = true;
                    emImage.enabled = false;
                }
                transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation = 
                    Quaternion.Euler(new Vector3(staticOriginalRotation.x, staticOriginalRotation.y, staticOriginalRotation.z)), 1f * Time.deltaTime);
            }
        }
        else if (staticRotate && !agent.pathPending && agent.remainingDistance < gap)
        {
            anim.SetInteger("State", 0);
            StaticRaycastProperties();
            agent.angularSpeed = 35;
            if (spotterIntervals == 0)
            {
                transform.Rotate(0, Time.deltaTime * agent.angularSpeed, 0);
            }
            else if (spotterIntervals == 1)
            {
                transform.Rotate(0, Time.deltaTime * -agent.angularSpeed, 0);
            }
        }
    }

    void Chase()
    {
        if(specialType == 0)
        {
            agent.speed = 8.5f;
            agent.angularSpeed = 700;
            agent.acceleration = 100;
            if (Vector3.Distance(thisAI.position, playerTarget.position) < maxRadius)
            {
                anim.SetInteger("State", 1);
                agent.SetDestination(playerTarget.position); //uses nav mesh to chase after target
                lastSeen = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
                playerOutline.transform.parent = null;
                playerOutline.SetActive(false);
            }
            else if (Vector3.Distance(thisAI.position, playerTarget.position) > maxRadius)
            {
                playerOutline.SetActive(true);
                playerOutline.transform.position = lastSeen;
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    qmImage.enabled = false;
                    emImage.enabled = false;
                    playerOutline.SetActive(false);
                    isInFov = false;
                    enemyType = originalType;
                }
            }
        }
        else if (specialType == 1)
        {
            Vector3 targetDir = playerTarget.position - transform.position;
            float step = 1.85f * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);

            if (Vector3.Distance(thisAI.position, playerTarget.position) < maxRadius)
            {
                lastSeen = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
                playerOutline.transform.position = lastSeen;
                foreach (ArtificialIntelligence ai in callForThugs)
                {
                    ai.visualState = ai.exclamationMark;
                    ai.qmImage.enabled = false;
                    ai.emImage.enabled = true;
                    ai.agent.SetDestination(playerTarget.position);
                    ai.isInFov = true;
                    ai.lastSeen = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
                    ai.playerOutline.transform.position = ai.lastSeen;
                    //ai.enemyType = 3;
                    //ai.noiseTarget = playerTarget.transform;
                }
            }
            else if (Vector3.Distance(thisAI.position, playerTarget.position) > maxRadius)
            {
                qmImage.enabled = false;
                emImage.enabled = false;
                playerOutline.SetActive(true);
                isInFov = false;
                enemyType = originalType;
            }
        }
    }

    void FOVProperties()
    {
        Vector3 directionBetween = (playerTarget.position - thisAI.position).normalized;
        directionBetween.y *= 0; //height difference is able to influence its angle, it makes height is not a factor

        float angle = Vector3.Angle(thisAI.forward, directionBetween); //ensures chasing only resumes when it is within the AI's view

        if (angle <= maxAngle)
        {
            Ray ray = new Ray(thisAI.position, playerTarget.position - thisAI.position); //ensures the raycast is resting from this AI
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxRadius) && hit.transform == playerTarget)
            {
                print("hit");
                isInFov = true;
                visualState = exclamationMark;
                qmImage.enabled = false;
                emImage.enabled = true;
            }
        }
    }

    void StaticRaycastProperties()
    {
        var up = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        Debug.DrawRay(transform.position, up * raycastLength, Color.white);

        if (Physics.Raycast(transform.position, up, out hit, raycastLength))
        {
            if (hit.transform.name == lookA.transform.name)
            {
                spotterIntervals = 1;
            }
            if (hit.transform.name == lookB.transform.name)
            {
                spotterIntervals = 0;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Noisemaker")
        {
            if (other.GetComponent<Noisemaker>().active && !isInFov && specialType ==0)
            {
                visualState = questionMark;
                qmImage.enabled = true;
                emImage.enabled = false;
                print("got it");
                noiseTarget = other.transform;
                enemyType = 3;
            }
        }
    }

    void CheckNoise()
    {
        //if (noiseTarget != null)
        //{
        //agent.SetDestination(noiseTarget.position);
        if (!isInFov)
        {
            StartCoroutine(Suspicious());
        }
        if (agent.remainingDistance < gap )
        {
            anim.SetInteger("State", 0);
            agent.speed = 0;
            agent.SetDestination(transform.position);
            timer = timer + Time.deltaTime;
            if (timer >= 5f)
            {
                agent.speed = 3;
                enemyType = originalType;
                noiseTarget = null;
                qmImage.enabled = false;
                emImage.enabled = false;
                playerOutline.SetActive(false);
                anim.SetInteger("State", 1);
                timer = 0;
            }
        }
        //}
    }

    IEnumerator Suspicious()
    {
        anim.SetInteger("State", 0);
        agent.speed = 0;
        Vector3 targetDir = noiseTarget.position - transform.position;
        float step = 3.5f * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
        yield return new WaitForSeconds(3.5f);
        agent.SetDestination(noiseTarget.position);
        if (agent.remainingDistance > gap)
        {
            anim.SetInteger("State", 1);
        }
        agent.speed = 3;
        print("aiming");
    }

    #region gizmos
    private void OnDrawGizmos() //the max angle determines how wide its fov will be based on the blue lines and the max radius determines how far will the fov be based on the yellow sphere
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius);

        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * maxRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * maxRadius; //Ensures the second blue fov line goes the other angle

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        if (!isInFov)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, (playerTarget.position - transform.position).normalized * maxRadius); //ensures the middle raycasting line turns to green when hitting the target

        //Gizmos.color = Color.black;
        //Gizmos.DrawRay(transform.position, transform.forward * maxRadius);
    }
    #endregion
}
