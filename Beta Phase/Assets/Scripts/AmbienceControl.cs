using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceControl : MonoBehaviour
{
    public AudioSource Ambience1,Ambience2,Ambience3,Ambience4;
    string  vent, bgm, drip, owl;
    //public float dripTimer, owlTimer;

    private void Start()
    {
        vent = "Vent";
        bgm = "night ambience";
        drip = "Water drip";
        owl = "Owl";

        Ambience1.clip = (AudioClip)Resources.Load(vent);
        Ambience2.clip = (AudioClip)Resources.Load(bgm);
        Ambience3.clip = (AudioClip)Resources.Load(drip);
        Ambience4.clip = (AudioClip)Resources.Load(owl);

        VentAndBgm();

        InvokeRepeating("Drip", 0f, 12f);
        InvokeRepeating("Owl", 0f, 16f);

    }

    void VentAndBgm()
    {
        if (Ambience1 != null || Ambience2 != null)
        {
            Ambience1.Play();
            Ambience2.Play();
        }
        else return;
    }

    void Drip()
    {
        if (Ambience3 != null)
        {
            Ambience3.Play();
        }
        else return;
    }

    void Owl()
    {
        if (Ambience4 != null)
        {
            Ambience4.Play();
        }
        else return;    
    }

    void Update()
    {
        //for Ambient and Vent
        

        //for Drip loop
        //if(dripTimer >= 0)
        //{
        //    dripTimer -= Time.deltaTime;
        //}
        //if(!Ambient3.isPlaying &&dripTimer <= 0)
        //{
        //    drip = "Water drip";
        //    Ambient3.clip = (AudioClip)Resources.Load(drip);
        //    Ambient3.Play();
            
        //}
        //if(dripTimer <= 0)
        //{
        //    dripTimer = 12f;
        //}

        //for Owl loop
        //if (owlTimer >= 0)
        //{
        //    owlTimer -= Time.deltaTime;
        //}
        //if (!Ambient4.isPlaying && owlTimer <= 0)
        //{
        //    owl = "Owl";
        //    Ambient4.clip = (AudioClip)Resources.Load(owl);
        //    Ambient4.Play();

        //}
        //if (owlTimer <= 0)
        //{
        //    owlTimer = 16f;
        //}
    }
}
