using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class OpenDoors : MonoBehaviour {

    bool canBeOpen;
    public Text pressE;
    public NavMeshObstacle obstacle;
    float timer;
    AudioSource externalAudio;
    Outline2 outline2;
    // Use this for initialization
    void Start () {
        externalAudio = GetComponent<AudioSource>();
        outline2 = gameObject.transform.GetChild(0).GetComponent<Outline2>();
    }
	
	// Update is called once per frame
	void Update () {
        if (canBeOpen)
        {
            timer += Time.deltaTime;
            if (timer < 0.5f)
            {
                transform.Rotate(0, 0, +3);
            }
            else if (timer > 0.5f)
            {
                transform.Rotate(0, 0, 0);
                obstacle.enabled = false;
            }
        }
	}

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            if (!canBeOpen)
            {
                pressE.text = "Press E to open";
                pressE.enabled = true;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                externalAudio.clip = (AudioClip)Resources.Load("OpeningDoor");
                externalAudio.Play();
                canBeOpen = true;
                pressE.enabled = false;
                outline2.enabled = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            pressE.enabled = false;
        }
    }
}
