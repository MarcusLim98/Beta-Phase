using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ItemPickUp : MonoBehaviour {

    public Text pressE;
	public bool haveBottle;
    public Image bottleFilled;
    public string fileName, pickUpFileName;
    AudioSource externalAudio, pickUpSound;

    private void Start()
    {
        externalAudio = GameObject.Find("Playershoot").GetComponent<AudioSource>();
        externalAudio.clip = (AudioClip)Resources.Load(fileName);
        pickUpSound = GameObject.Find("PickUpSound").GetComponent<AudioSource>();
        pickUpSound.clip = (AudioClip)Resources.Load(pickUpFileName);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name.Contains("GlassBottle") && !haveBottle)
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
        if (other.CompareTag("KeyItem"))
        {
            pressE.enabled = true;
            if (Input.GetKeyDown(KeyCode.E))
            {
                pressE.enabled = false;
                pickUpSound.Play();
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("GlassBottle") && !haveBottle || other.CompareTag("KeyItem"))
        {
            pressE.enabled = false;
        }
    }
}
