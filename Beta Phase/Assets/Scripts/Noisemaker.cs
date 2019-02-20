using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Noisemaker : MonoBehaviour {

    public Text pressE;
    public Rigidbody rb;

    public ArtificialIntelligence[] ai;
    public Vector3[] movePaths;
    public Vector3[] whereToLook;
    public bool hasInteracted, mustInteract;
    AudioSource externalAudio;
    string fileName;
    public string progressName;

    private void Start()
    {
        externalAudio = GetComponent<AudioSource>();
        fileName = "CrateSmash";

        if (progressName != null && PlayerPrefs.GetInt(progressName) >= 1)
        {
            Invoke("MoveAi", 0f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !mustInteract && !hasInteracted)
        {
            pressE.text = "Press E to interact";
            if (Input.GetKeyDown(KeyCode.E))
            {
                hasInteracted = true;
                rb.AddForce(transform.up * 1000f);
                MoveAi();
            }
            if (!hasInteracted)
            {
                pressE.enabled = true;
            }
            else if (hasInteracted)
            {
                pressE.enabled = false;
                SoundFX();
            }
        }

        if (other.tag == "Player" && mustInteract)
        {
            MoveAi();
        }
    }

    void MoveAi()
    {
        for (int i = 0; i < movePaths.Length; i++)
        {
            ai[i].stationeryPosition.position = movePaths[i];
            ai[i].questionMark.SetActive(true);
        }
        for (int i = 0; i < whereToLook.Length; i++)
        {
            ai[i].lookHereStart = whereToLook[i];
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && !hasInteracted)
        {
            pressE.enabled = false;
        }
    }

    public void SoundFX()
    {
        if (!externalAudio.isPlaying)
        {
            externalAudio.volume = 1;
            externalAudio.PlayOneShot((AudioClip)Resources.Load(fileName), 1f);
        }
    }
}
