using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AISpotter : MonoBehaviour {

    public Transform playerTarget;
    public Transform playerHighlight;
    public Transform noisySource;
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
    Transform thisAI, startingAngle;
    GameObject EmptyObj;
    int investigatingState, isInFov;
    // Use this for initialization
    void Start()
    {
        EmptyObj = new GameObject("Look Here");
        EmptyObj.layer = 9;
        EmptyObj.transform.parent = this.gameObject.transform;
        startingAngle = this.gameObject.transform.GetChild(3);
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
        Vector3 directionBetween = (playerTarget.position - thisAI.position).normalized;
        directionBetween.y *= 0; //height difference is able to influence its angle, it makes height is not a factor

        angle = Vector3.Angle(thisAI.forward, directionBetween); //ensures chasing only resumes when it is within the AI's view

        if (angle <= maxAngle)
        {
            //print("within range");
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

                    foreach (ArtificialIntelligence ai in thugsToCall)
                    {
                        ai.spottedHighlight = true;
                        playerHighlight.transform.parent = playerTarget;
                        playerHighlight.transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
                        playerHighlight.transform.parent = null;
                    }
                    print("investigating");
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
}
