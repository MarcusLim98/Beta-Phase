using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkSoundTest : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] AudioSource footSource;
    Animator thug;
    string right, left;
    public bool Rstop, Lstop;
    void Start()
    {
        footSource = GetComponent<AudioSource>();
        thug = GetComponent<Animator>();
        right = "ThugWalkR";
        left = "ThugWalkL";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RightFoot()
    {
        footSource.volume = 0.2f;
        footSource.clip = (AudioClip)Resources.Load(right);
        footSource.Play();
        Rstop = false;
    }
    public void LeftFoot()
    {
        footSource.volume = 0.2f;
        footSource.clip = (AudioClip)Resources.Load(left);
        footSource.Play();
        Lstop = false;
    }
    public void FootStop()
    {
        if (Rstop == false || Lstop == false)
        {
            footSource.Stop();
            Rstop = true;
            Lstop = true;
        }
    }
}
