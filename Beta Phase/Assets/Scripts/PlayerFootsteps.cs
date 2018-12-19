using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour {

    public AudioSource externalAudio;
    string fileName;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Path")
        {
            fileName = "SingleStep";
            externalAudio.clip = (AudioClip)Resources.Load(fileName);
            externalAudio.Play();
        }
    }
}
