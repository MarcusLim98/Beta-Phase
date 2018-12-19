using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;   

public class SpareAILogic : MonoBehaviour {

	 //Navmesh method
    public Transform playerTarget, noiseTarget; //ensures which game object it will be chasing
    public GameObject playerOutline;
    public int enemyType; //sets whether you want the ai to be patrolling or chasing (1 = patrolling, 2 = chases when player is within fov, 4 = only rotates but still chases player)
    public int specialType; //1 = throws bottle at player, 2 = investigates the spot where 1 has thrown while also finding yy, 3 = spotter
    public float gap; //a value for the agent to leave a gap between itself and the intended path it is travelling towards when changing its destination point
    public float maxAngle;
    public float maxRadius;
    public float Timer;
    public float sphereRadius;
    public float raycastLength;
    public int destPoint; //a value to keep track of which destination point
    public Vector3 lastSeen;
    public Transform lookA, lookB;
    public Transform[] points; //an array to store all of the destination point
    public ArtificialIntelligence[] callForThugs;
    NavMeshAgent agent;
    private Transform thisAI;
	private Vector3 sphereCenter;
	private Vector3 currentPosition;
    public bool isPoliceman, staticRotate;

    int originalType, spotterIntervals; 
    float timer;
    [HideInInspector]
    public bool isInFov = false, onStay = false, isInCollider = false, hasShotBottle;
    Animator anim;
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

            if (enemyType == 3 && !isPoliceman)
            {
                CheckNoise();
            }
            else if (enemyType == 1)
            {
                GotoNextPoint();
            }
            else if (enemyType == 2 && !isPoliceman)
            {
                RandomPoint();
            }
        }

        if (spotCheck.atNoiseSource)
        {
            timer = timer + Time.deltaTime;
        }
        else timer = 0f;
    }

    void GotoNextPoint() //Those red cubes are its destination points but an empty game object works fine as well
    {
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

        if (!staticRotate)
        {
            anim.SetInteger("State", 1);
        }
        else if (staticRotate && !agent.pathPending && agent.remainingDistance < gap)
        {
            anim.SetInteger("State", 0);
            agent.angularSpeed = 35;
            if (spotterIntervals == 0)
            {
                transform.Rotate(0, Time.deltaTime * agent.angularSpeed, 0);
            }
            else if (spotterIntervals == 1)
            {
                transform.Rotate(0, Time.deltaTime * -agent.angularSpeed, 0);
            }
            StaticRaycastProperties();
        }
    }

    void RandomPoint()
    {
        if (points.Length == 0)
            return;

        agent.destination = points[destPoint].position;
        Vector3 destinPos = agent.destination - transform.position; // update the postion and rotation when ai moves
        //Vector3 lookPos = lookHere.position - transform.position;

        //Select a random point in the array
        //Travel towards that array
        if (!agent.pathPending && agent.remainingDistance < gap && onStay == false)
        {
            destPoint = ((Random.Range(0, points.Length) % points.Length));
            //look at current destination and rotate towards it
            //Quaternion rotation = Quaternion.LookRotation(destinPos, Vector3.up);
            //transform.rotation = rotation;

        }
        if (agent.remainingDistance >= 0 && onStay == false)  //when player is no longer walking and just arrived at destination
        {
            Timer = Random.Range(0, 8); //random time for which guard stations them selves
            onStay = true;
        }
        if (agent.remainingDistance <= 0 && onStay == true)
        {
            //look at lookHere after moving
            //Quaternion rotation = Quaternion.LookRotation(lookPos, Vector3.up);
            //transform.rotation = rotation;
            Timer -= Time.deltaTime;
        }
        if (Timer <= 0 && onStay == true)
        {
            //end the cycle
            onStay = false;
        }

    }

    void Chase()
    {
        if (Vector3.Distance(thisAI.position, playerTarget.position) < maxRadius && specialType !=3)
        {
            anim.SetInteger("State", 1);
            agent.speed = 10;
            agent.angularSpeed = 700;
            agent.acceleration = 100;
            agent.SetDestination(playerTarget.position); //uses nav mesh to chase after target
            lastSeen = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
            playerOutline.transform.parent = null;
            playerOutline.SetActive(false);
        }
        else if (Vector3.Distance(thisAI.position, playerTarget.position) > maxRadius)
        {
            agent.SetDestination(lastSeen);
            playerOutline.SetActive(true);
            playerOutline.transform.position = lastSeen;
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                anim.SetInteger("State", 0);
                agent.speed = 3;
                agent.angularSpeed = 200;
                agent.acceleration = 8f;
                playerOutline.transform.parent = GameObject.FindGameObjectWithTag("Player").transform;
                //playerOutline.transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
                playerOutline.SetActive(false);
                isInFov = false;
                enemyType = originalType;
            }
        }

        if (specialType == 2)
        {
            UpdatingWaypoints();
        }
        else if (specialType == 3)
        {
            Vector3 targetDir = playerTarget.position - transform.position;
            // The step size is equal to speed times frame time.
            float step = 1.85f * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            // Move our position a step closer to the target.
            transform.rotation = Quaternion.LookRotation(newDir);

            /*foreach (ArtificialIntelligence ai in callForThugs)
            {
                ai.lastSeen = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
                ai.isInFov = true;
                ai.agent.speed = 10;
                ai.agent.angularSpeed = 700;
                ai.agent.acceleration = 100;
            }*/
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
                isInFov = true;
                print("spotted");
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
                print("hit");
                spotterIntervals = 1;
            }
            if (hit.transform.name == lookB.transform.name)
            {
                spotterIntervals = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Noisemaker>().active)
        {
            if (specialType == 0 || specialType == 2)
            {
                noiseTarget = other.transform;
                enemyType = 3;
            }
            if (specialType == 1 && !hasShotBottle)
            {
                bottleThrow.playerBottle = this.gameObject.transform.GetChild(0);
                bottleThrow.notPlayer = true;
                bottleThrow.enabled = true;
                hasShotBottle = true;
                if (!isInCollider)
                {
                    hasShotBottle = false;
                }
            }
        }
    }

    void CheckNoise()
    {
        //print("check noise1");
        StartCoroutine(Suspicious());

        if (agent.remainingDistance < gap)
        {
            anim.SetInteger("State", 0);
            //print("near bottle");
            //agent.SetDestination(transform.position);
        }
        if (timer >= 5f)
        {
            //print("check noise3");
            noiseTarget = null;
            enemyType = originalType;
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

        if (!isInFov)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, (playerTarget.position - transform.position).normalized * maxRadius); //ensures the middle raycasting line turns to green when hitting the target

		if (specialType == 2)
		{
				Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, sphereRadius);
		}
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && specialType == 1 && bottleThrow.enabled == true) //prevents a bug that causes the AI to spam throwing bottles when player is within its collider
        {
            isInCollider = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && specialType == 1 && bottleThrow.enabled == false)
        {
            isInCollider = false;
            hasShotBottle = false; 
        }
    }

    IEnumerator Suspicious()
    {
        anim.SetInteger("State", 0);
        agent.SetDestination(noiseTarget.position);
        agent.speed = 0;
        Vector3 targetDir = noiseTarget.position - transform.position;
        float step = 1.85f * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
        yield return new WaitForSeconds(2f);
        anim.SetInteger("State", 1);
        agent.speed = 3;
    }

    void UpdatingWaypoints()
    {
        /*Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereRadius);
		foreach (Collider hit in hitColliders){
			if(hit.tag == "Waypoints"){
				print(hit.transform.name);	
			    for (int i = 0; i < points.Length; ++i)
				{
				   points[i] = hit.transform ;
				}
			}
		}*/
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.transform.name.Contains("Apath"))
            {
                for (int i = 0; i < points.Length; ++i)
                {
                    points[i] = GameObject.Find("PathSet1").GetComponentInChildren<Transform>().GetChild(i);
                }
            }
            else if (hit.transform.name.Contains("Bpath"))
            {
                for (int i = 0; i < points.Length; ++i)
                {
                    points[i] = GameObject.Find("PathSet2").GetComponentInChildren<Transform>().GetChild(i);
                }
            }
            else if (hit.transform.name.Contains("Cpath"))
            {
                for (int i = 0; i < points.Length; ++i)
                {
                    points[i] = GameObject.Find("PathSet3").GetComponentInChildren<Transform>().GetChild(i);
                }
            }
            else if (hit.transform.name.Contains("Dpath"))
            {
                for (int i = 0; i < points.Length; ++i)
                {
                    points[i] = GameObject.Find("PathSet4").GetComponentInChildren<Transform>().GetChild(i);
                }
            }
        }
    }
}
