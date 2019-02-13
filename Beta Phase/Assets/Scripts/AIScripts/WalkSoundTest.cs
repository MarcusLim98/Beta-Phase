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
        right = "ThugWalkR";
        left = "ThugWalkL";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void RightFoot()
    {
        footSource.clip = (AudioClip)Resources.Load(right);
        footSource.Play();
        Rstop = false;
    }
    void LeftFoot()
    {
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
