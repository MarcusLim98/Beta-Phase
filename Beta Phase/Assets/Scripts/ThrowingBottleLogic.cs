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
    public LayerMask layer;
    public float forceToThrow, maxRadius, limit;

    Transform launchFrom;
    Vector3 target, toTarget, aimingWhere, Vo;
    PlayerLogic playerLogic;
    ItemPickUp itemPickUp;
    float dist;
    private int numPoints = 50;
    private Vector3[] positions = new Vector3[50];
    private Camera cam;
    // Use this for initialization
    void Start()
    {
        playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();
        itemPickUp = GameObject.Find("Player").GetComponent<ItemPickUp>();
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
            if(dist< limit)
            {
                lr.enabled = true;
                cursor.SetActive(true);
            }
            else if (dist > limit)
            {
                lr.enabled = false;
                cursor.SetActive(false);
            }

            cursor.transform.position = hit.point + Vector3.up * 0.1f;
            aimingWhere = hit.point;
            Vo = CalculateVelocity(hit.point, launchFrom.position, forceToThrow);
            launchFrom.transform.rotation = Quaternion.LookRotation(Vo);
            midPoint.rotation = Quaternion.LookRotation(Vo);

            if (Input.GetMouseButtonDown(0) && dist < limit)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius);
    }
}
