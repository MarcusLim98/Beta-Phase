using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceControl : MonoBehaviour
{
    public AudioSource Ambient1,Ambient2,Ambient3,Ambient4;
    string  vent , bgm , drip , owl;
    public float dripTimer, owlTimer;
    public bool dripStart , owlStart;

    // Start is called before the first frame update
    void Start()
    {
        dripStart = false;
        owlStart = false;
    }

    // Update is called once per frame
    void Update()
    {
        //for Ambient and Vent
        if (!Ambient1.isPlaying || !Ambient2.isPlaying)
        {
            vent = "Vent";
            bgm = "night ambience";
            Ambient1.clip = (AudioClip)Resources.Load(vent);
            Ambient2.clip = (AudioClip)Resources.Load(bgm);
            Ambient1.PlayOneShot(Ambient1.clip , 0.2f);
            Ambient2.PlayOneShot(Ambient2.clip , 0.5f);
        }
        //for Drip loop
        if(dripTimer >= 0)
        {
            dripTimer -= Time.deltaTime;
        }
        if(!Ambient3.isPlaying &&dripTimer <= 0)
        {
            drip = "Water drip";
            Ambient3.clip = (AudioClip)Resources.Load(drip);
            Ambient3.Play();
            
        }
        if(dripTimer <= 0)
        {
            dripTimer = 12f;
        }
        //for Owl loop
        if (owlTimer >= 0)
        {
            owlTimer -= Time.deltaTime;
        }
        if (!Ambient4.isPlaying && owlTimer <= 0)
        {
            owl = "Owl";
            Ambient4.clip = (AudioClip)Resources.Load(owl);
            Ambient4.Play();

        }
        if (owlTimer <= 0)
        {
            owlTimer = 16f;
        }
    }
}
