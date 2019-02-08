using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkSoundTest : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource footSource;
    string right, left;
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
    }
    void LeftFoot()
    {
        footSource.clip = (AudioClip)Resources.Load(left);
        footSource.Play();
    }
}
