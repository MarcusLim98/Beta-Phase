using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaoDaBullet : MonoBehaviour {

    public float speed;
    Transform player;
	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player").transform;
        //transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        transform.LookAt(player.position);
    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(0, 0, speed * Time.deltaTime);
    }
}
