using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AISpotter : MonoBehaviour {

    public Transform playerTarget;
    public Transform playerHighlight;
    public Transform noisySource;
    public GameObject alert;
    public LayerMask layerMask, layerMask2;
    [Space]
    [Space]
    public bool turnBack;
    public bool canRotateLoop = true;
    public float maxRadius, maxAngle, rotatingSpeed, angle;
    public Vector3 moveEmptyObj;
    [Space]
    [Space]
    public ArtificialIntelligence[] thugsToCall;
    NavMeshAgent agent;
    Animator anim;
    Transform thisAI, startingAngle, uiAbove;
    Vector3 directionBetween;
    GameObject EmptyObj, exclamationMark;
    bool playerWithinRadius;
    PlayerLogic playerLogic;
    int investigatingState, isInFov;
    // Use this for initialization
    void Start()
    {
        uiAbove = this.gameObject.transform.GetChild(4);
        exclamationMark = Instantiate(alert, transform.position, Quaternion.identity);
        exclamationMark.transform.position = new Vector3(uiAbove.position.x, uiAbove.position.y, uiAbove.position.z);

        playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();
        EmptyObj = new GameObject("Look Here");
        EmptyObj.layer = 9;
        EmptyObj.transform.parent = this.gameObject.transform;
        startingAngle = this.gameObject.transform.GetChild(5);
        SphereCollider sc = EmptyObj.AddComponent<SphereCollider>() as SphereCollider;
        sc.radius = 0.5f;
        sc.isTrigger = true;
        EmptyObj.transform.parent = null;
        EmptyObj.transform.position = new Vector3(transform.position.x + moveEmptyObj.x, transform.position.y, transform.position.z + moveEmptyObj.z);

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        thisAI = GetComponent<Transform>();
        anim.SetInteger("State", 0);
    }

    // Update is called once per frame
    void Update()
    {
        InFov();
        StationeryRotation();
    }
    void StationeryRotation()
    {
        var up = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        Debug.DrawRay(transform.position, up * 10f, Color.green);

        if (Physics.Raycast(transform.position, up, out hit, 10f, layerMask2))
        {
            if (investigatingState == 0 && canRotateLoop)
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
            else if (investigatingState == 0 && !canRotateLoop)
            {
                if (hit.transform.name == "Look Here")
                {
                    rotatingSpeed = 30f;
                    canRotateLoop = true;
                    exclamationMark.SetActive(false);
                }
            }
        }

        if (!turnBack && investigatingState == 0)
        {
            transform.Rotate(0, Time.deltaTime * rotatingSpeed, 0);
        }
        else if (turnBack && investigatingState == 0)
        {
            transform.Rotate(0, Time.deltaTime * -rotatingSpeed, 0);
        }
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

            if (Physics.Raycast(ray, out hit, maxRadius, layerMask))
            {
                if (hit.transform == playerTarget)
                {
                    investigatingState = 1;
                    canRotateLoop = false;
                    var lookPos = playerTarget.position - transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotatingSpeed);
                    exclamationMark.SetActive(true);

                    foreach (ArtificialIntelligence ai in thugsToCall)
                    {
                        ai.spottedHighlight = true;
                        playerHighlight.transform.parent = playerTarget;
                        playerHighlight.transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
                        playerHighlight.transform.parent = null;
                    }
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

        if (investigatingState == 0 && !canRotateLoop)
        {
            rotatingSpeed = 5f;
            var lookPos = startingAngle.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotatingSpeed);
        }
        return false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "NoisyFloor")
        {
            if (playerLogic.stepOnNoisyFloor == true && playerWithinRadius == true)
            {
                canRotateLoop = false;
                var lookPos = playerTarget.position - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotatingSpeed);
                exclamationMark.SetActive(true);

                foreach (ArtificialIntelligence ai in thugsToCall)
                {
                    ai.spottedHighlight = true;
                    playerHighlight.transform.parent = playerTarget;
                    playerHighlight.transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
                    playerHighlight.transform.parent = null;
                }
            }
        }

        if (other.tag == "Player")
        {
            playerWithinRadius = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerWithinRadius = false;
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
}
