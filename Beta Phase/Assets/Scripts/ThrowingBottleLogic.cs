using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowingBottleLogic : MonoBehaviour {

    public Rigidbody rb;
    public GameObject cursor;
    public LineRenderer lr;
    public Transform midPoint;
    public Image bottleIcon;
    public LayerMask layer, layer2;
    public float forceToThrow;//, limit;
    public bool hitSomething;

    Transform launchFrom;
    Vector3 target, toTarget, aimingWhere, Vo;
    PlayerLogic playerLogic;
    ItemPickUp itemPickUp;
    ParticleSystem.MainModule ps;
    float dist;
    private int numPoints = 50;
    private Vector3[] positions = new Vector3[50], newPointsInLine = null;
    private Camera cam;
    // Use this for initialization
    void Start()
    {
        playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();
        itemPickUp = GameObject.Find("Player").GetComponent<ItemPickUp>();
        ps = cursor.transform.GetChild(0).GetComponent<ParticleSystem>().main;
        cam = Camera.main;
        launchFrom = GetComponent<Transform>();
        lr = GetComponent<LineRenderer>();
        lr.positionCount = numPoints;
    }

    // Update is called once per frame
    void Update()
    {
        LaunchProjectile();
    }

    void LaunchProjectile()
    {
        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetKey(KeyCode.W) && Physics.Raycast(camRay, out hit, 100f, layer) && itemPickUp.haveBottle == true)
        {
            dist = Vector3.Distance(cursor.transform.position, transform.position);
            playerLogic.noMoving = true;
            if(dist< 15f)
            {
                lr.enabled = true;
                cursor.SetActive(true);
            }
            else if (dist > 15f)
            {
                lr.enabled = false;
                cursor.SetActive(false);
            }

            if (dist > 8f)
            {
                forceToThrow = 1.2f;
            }
            else if (dist < 8f)
            {
                forceToThrow = 0.5f;
            }

            cursor.transform.position = hit.point + Vector3.up * 0.1f;
            aimingWhere = hit.point;
            Vo = CalculateVelocity(hit.point, launchFrom.position, forceToThrow);
            launchFrom.transform.rotation = Quaternion.LookRotation(Vo);
            midPoint.rotation = Quaternion.LookRotation(Vo);

            if (Input.GetMouseButtonDown(0) && dist < 15f && hitSomething)
            {
                Rigidbody obj = Instantiate(rb, launchFrom.position, Quaternion.identity);
                obj.velocity = Vo;
                itemPickUp.haveBottle = false;
                bottleIcon.enabled = false;
            }
        }
        else
        {
            cursor.SetActive(false);
            lr.enabled = false;
            playerLogic.noMoving = false;
        }

        if (hitSomething)
        {
            //use new points for the line renderer
            lr.positionCount = newPointsInLine.Length;

            lr.SetPositions(newPointsInLine);
            lr.startColor = new Color(1, 1, 1,0);
            lr.endColor = new Color(1, 1, 1, 1);
            ps.startColor = new Color(1, 1, 1, 1);
        }
        else
        {
            //use old points for the line renderer
            lr.positionCount = positions.Length;

            lr.SetPositions(positions);
            lr.startColor = new Color(1, 0, 0, 0); 
            lr.endColor = new Color(1, 0, 0, 1);
            ps.startColor = new Color(1, 0, 0, 1);
        }
    }

    Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
    {
        //define the distance of x and y first
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0f;

        //create a float to represent our distance
        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;

        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;

        for (int i = 1; i < numPoints + 1; i++)
        {
            float t = i / (float)numPoints;
            positions[i - 1] = CalculateQuadraticBezierPoint(t, launchFrom.position , midPoint.position, aimingWhere);
        }

        lr.SetPositions(positions);

        RaycastHit hitInfo;
        for (int i = 0; i < positions.Length - 1; i++)
        {
            if (Physics.Linecast(positions[i], positions[i + 1], out hitInfo))
            {
                //initialize the new array to the furthest point + 1 since the array is 0-based
                newPointsInLine = new Vector3[(i + 1) + 1];

                //transfer the points we need to the new array
                for (int i2 = 0; i2 < newPointsInLine.Length; i2++)
                {
                    newPointsInLine[i2] = positions[i2];
                }

                //set the current point to the raycast hit point (the end of the line renderer)
                newPointsInLine[i + 1] = hitInfo.point;

                //flag that we hit something
                if (hitInfo.transform.tag == "Path")
                {
                    hitSomething = true;
                }
                else hitSomething = false;

                break;
            }
        }

        return result;
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }
}
