using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkSoundTest : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource footSource;
    string right, left;
    public bool Rstop, Lstop;
    void Start()
    {
        footSource = GetComponent<AudioSource>();
        right = "ThugWalkR";
        left = "ThugWalkL";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void RightFoot()
    {
        footSource.volume = 0.05f;
        footSource.clip = (AudioClip)Resources.Load(right);
        footSource.Play();
        Rstop = false;
    }
    void LeftFoot()
    {
        footSource.volume = 0.05f;
        footSource.clip = (AudioClip)Resources.Load(left);
        footSource.Play();
        Lstop = false;
    }
    void StopFoot()
    {
        if (Rstop == false || Lstop == false)
        {
            footSource.Stop();
            Rstop = true;
            Lstop = true;
        }
    }
}
