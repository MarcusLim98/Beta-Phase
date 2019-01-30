using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceControl : MonoBehaviour
{
    public AudioSource Ambient;
    string  vent;
    public float dripTimer = 10f;
    public bool dripStart;
    // Start is called before the first frame update
    void Start()
    {
        Ambient = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Ambient.isPlaying)
        {
            vent = "Vent";
            Ambient.clip = (AudioClip)Resources.Load(vent);

            Ambient.Play();
        }
    }
}
