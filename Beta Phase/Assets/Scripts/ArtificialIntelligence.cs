﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class ArtificialIntelligence : MonoBehaviour
{
    enum AIState { PATROLLING, INVESTIGATING, CHASE }; // States
    AIState state;

    public Transform playerTarget;
    public Transform playerHighlight;
    public Transform noisySource;
    public Transform stationeryPosition;
    public LayerMask layerMask;
    [Space]
    [Space]
    public AIPath aiPath;
    public float maxRadius, maxRadius2, maxAngle, maxAngle2, rotatingSpeed;
    public bool spottedHighlight, goToNoisySource, stationery, staticRotate;
    [Space]
    [Space]
    NavMeshAgent agent;
    Animator anim;
    Transform target, thisAI;
    Vector3 lookHereStart;
    GameObject EmptyObj;
    Outline2 playerOutline;
    int destPoint = 0, investigatingState, isInFov;
    float stopToLook, stopToGoBack, angle, startToTurn;
    bool turnBack, cannotTurn;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        thisAI = GetComponent<Transform>();
        playerOutline = playerHighlight.GetComponent<Outline2>();
        if (stationery)
        {
            EmptyObj = new GameObject("Look Here");
            EmptyObj.transform.parent = this.gameObject.transform;
            EmptyObj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            stationeryPosition = this.gameObject.transform.GetChild(5);
            EmptyObj.transform.parent = null;
            lookHereStart = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        }
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
                            agent.SetDestination(playerHighlight.position);
                            if (stopToLook <= 1.5f)
                            {
                                anim.SetInteger("State", 0);
                                playerHighlight.transform.parent = null;
                                //playerOutline.enabled = true;
                                agent.speed = 0;
                                Vector3 targetDir = playerHighlight.position - thisAI.position;
                                float step = 1.85f * Time.deltaTime;
                                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
                                transform.rotation = Quaternion.LookRotation(newDir);
                            }
                            else if (stopToLook >= 1.5f)
                            {
                                anim.SetInteger("State", 1);
                                agent.speed = 2f;
                            }
                            CheckAndReturn();
                        }
                        else if (isInFov == 2)
                        {
                            anim.SetInteger("State", 1);
                            agent.speed = 4f;
                            playerHighlight.transform.parent = null;
                            //playerOutline.enabled = true;
                            agent.SetDestination(playerHighlight.position);
                            CheckAndReturn();
                        }
                    }
                    else if (goToNoisySource && !spottedHighlight)
                    {
                        stopToLook += Time.deltaTime;
                        agent.SetDestination(noisySource.position);
                        if (stopToLook <= 1.5f)
                        {
                            anim.SetInteger("State", 0);
                            agent.speed = 0;
                            Vector3 targetDir = noisySource.position - thisAI.position;
                            float step = 1.85f * Time.deltaTime;
                            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
                            transform.rotation = Quaternion.LookRotation(newDir);
                        }
                        else if (stopToLook >= 1.5f)
                        {
                            anim.SetInteger("State", 1);
                            agent.speed = 2f;
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
                    playerHighlight.transform.parent = playerTarget;
                    playerHighlight.transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z);
                    if (isInFov == 1)
                    {
                        playerOutline.enabled = false;
                        playerHighlight.transform.parent = null;
                        agent.SetDestination(playerHighlight.position);
                        stopToLook += Time.deltaTime;
                        if (stopToLook <= 1.5f)
                        {
                            anim.SetInteger("State", 0);
                            playerHighlight.transform.parent = null;
                            agent.speed = 0;
                            Vector3 targetDir = playerHighlight.position - thisAI.position;
                            float step = 1.85f * Time.deltaTime;
                            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
                            transform.rotation = Quaternion.LookRotation(newDir);
                        }
                        else if (stopToLook >= 1.5f)
                        {
                            anim.SetInteger("State", 1);
                            agent.speed = 2f;
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
                    agent.speed = 4f;
                    state = AIState.PATROLLING;
                }
                else if (investigatingState == 1)
                {
                    agent.speed = 4f;
                }
                else if (investigatingState == 2)
                {
                    agent.speed = 4f;
                    anim.SetInteger("State", 1);
                    agent.SetDestination(playerTarget.position);
                    playerOutline.enabled = false;
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
            print("going back");
            anim.Play("Walk");
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
        playerOutline.enabled = true;
        if ((!goToNoisySource && spottedHighlight) || (goToNoisySource && spottedHighlight))
        {
            target = playerHighlight;
        }
        else if (goToNoisySource && !spottedHighlight)
        {
            target = noisySource;
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
                playerOutline.enabled = false;
                spottedHighlight = false;
                goToNoisySource = false;
                stopToGoBack = 0;
                stopToLook = 0;
                isInFov = 0;
                agent.speed = 2f;
                GotoNextPoint();
                print("go back to original position");
            }
        }
    }

    public bool InFov()
    {
        Vector3 directionBetween = (playerTarget.position - thisAI.position).normalized;
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
                    investigatingState = 1;
                    isInFov = 1;
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
                    return true;
                }
            }
        }
        return false;
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

        //Gizmos.color = Color.black;
        //Gizmos.DrawRay(transform.position, transform.forward * maxRadius);
    }
}
