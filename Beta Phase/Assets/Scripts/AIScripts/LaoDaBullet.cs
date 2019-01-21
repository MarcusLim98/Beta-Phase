using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaoDaBullet : MonoBehaviour {

    public float speed;
    Transform player;
    AudioSource externalAudio;
    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player").transform;
        //transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        transform.LookAt(player.position);
        externalAudio = GetComponent<AudioSource>();
        externalAudio.clip = (AudioClip)Resources.Load("LaoDaGunShot");
        externalAudio.Play();
    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(0, 0, speed * Time.deltaTime);
    }
}
