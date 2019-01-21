using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSound : MonoBehaviour
{
    public string tagName, fileName;
    AudioSource externalAudio;
    private void Start()
    {
        externalAudio = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == tagName)
        {
            externalAudio.clip = (AudioClip)Resources.Load(fileName);
            externalAudio.Play();
        }
    }
}
