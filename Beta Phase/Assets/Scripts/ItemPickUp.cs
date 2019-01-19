using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ItemPickUp : MonoBehaviour {

    public Text pressE;
	public bool haveBottle;
    public Image bottleFilled;
    public string fileName;
    AudioSource externalAudio;
    private void Start()
    {
        externalAudio = GameObject.Find("Playershoot").GetComponent<AudioSource>();
        externalAudio.clip = (AudioClip)Resources.Load(fileName);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Bottle" && !haveBottle)
        {
            pressE.text = "Press E to interact";
            pressE.enabled = true;
            if (Input.GetKey(KeyCode.E))
            {
                bottleFilled.enabled = true;
                haveBottle = true;
                other.gameObject.SetActive(false);
                externalAudio.Play();
                pressE.enabled = false;
            }
        }
        if (other.CompareTag("SpawnPoint") || other.CompareTag("KeyItem"))
        {
            pressE.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Bottle" && !haveBottle)
        {
            pressE.enabled = false;
        }
        if (other.CompareTag("SpawnPoint") || other.CompareTag("KeyItem"))
        {
            pressE.enabled = false;
        }
    }
}
