using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public Transform player;
    public Vector3 targetOffset;
    public float movementSpeed;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Movement();
    }

    void Movement()
    {
        transform.position = Vector3.Lerp(transform.position, player.position + targetOffset, movementSpeed * Time.deltaTime);
    }
}
