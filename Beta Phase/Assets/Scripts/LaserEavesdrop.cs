using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEavesdrop : MonoBehaviour
{
    public float height, dist;
    public Color c1, c2, c3, c4;
    public int i;
    LineRenderer lr;
    Transform target;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //RaycastEavesdrop();
    }
     void RaycastEavesdrop()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider)
            {
                lr.SetPosition(1, new Vector3(0, 0, target.position.z));
            }
        }
        else
        {
            lr.SetPosition(1, new Vector3(0, 0, 0));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Thug")
        {
            lr.enabled = true;
            target = other.transform;
            lr.SetPosition(0, new Vector3(transform.position.x, transform.position.y + height, transform.position.z));
            lr.SetPosition(1, new Vector3(target.position.x, target.position.y + height, target.position.z));
            dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist > 10f)
            {
                i = 0;
            }
            else if (dist <= 7.5f && i == 0)
            {
                i = 1;
            }
            else if (dist <= 5.5f & i == 1)
            {
                i = 2;
            }
            else if (dist >= 5.5f & i == 2)
            {
                i = 3;
            }
            else if (dist >= 7.5f & i == 3)
            {
                i = 4;
            }

            if(i ==0 || i == 4)
            {
                lr.startColor = c1;
                lr.endColor = c1;
            }
            else if(i == 1 || i == 3)
            {
                lr.startColor = c2;
                lr.endColor = c2;
            }
            else if (i == 2)
            {
                lr.startColor = c3;
                lr.endColor = c3;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Thug")
        {
            i = 0;
            lr.enabled = false;
        }
    }
}
