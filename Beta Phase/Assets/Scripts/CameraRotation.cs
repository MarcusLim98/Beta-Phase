using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {

    public Transform center;
    public float speed;
    public bool orbit;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            orbit = true;
            speed = 50;
        }
        else if (Input.GetMouseButton(1))
        {
            orbit = false;
            speed = -50;
        }
        else speed = 0;
        transform.LookAt(center.position);
        transform.RotateAround(center.transform.position, new Vector3(0, 1, 0), Time.deltaTime * speed);
    }
}
