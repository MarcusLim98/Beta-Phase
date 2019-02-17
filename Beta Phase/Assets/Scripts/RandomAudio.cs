using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAudio : MonoBehaviour
{
    [SerializeField] AudioClip[] AudioClipArray;
    
    List<AudioClip> currentA = new List<AudioClip>();

    public bool inRange;

    AudioSource audioSource1 , audioSource2 , audioSource3;
    // Use this for initialization
    void Start()
    {
        //Loads in AudioClips from Resource/RandomAudios Folder
        AudioClipArray = Resources.LoadAll<AudioClip>("RandomAudios");
        
        audioSource1 = gameObject.transform.GetChild(0).GetComponent<AudioSource>();
        audioSource2 = gameObject.transform.GetChild(1).GetComponent<AudioSource>();
        audioSource3 = gameObject.transform.GetChild(2).GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        //when start eavesdropping
        /*if (Input.GetKeyDown(KeyCode.I))
        {
            inRange = true;
        }*/
            //play only when sound is not playing
            if ((!audioSource1.isPlaying || !audioSource2.isPlaying || !audioSource3.isPlaying) && inRange == true)
            {
            Source1();
            Source2();
            Source3();
            }
            //when stop eavesdropping
        /*if (Input.GetKeyDown(KeyCode.U))
        {
            inRange = false;
        }*/

        if (inRange == false)
        {
            AudioStop();
        }
        }
    //Pick a random sound to play until not in range. Applies for all 3
    void Source1()
    {
       
            audioSource1.clip = AudioClipArray[Random.Range(0, AudioClipArray.Length)];
            audioSource1.PlayOneShot(audioSource1.clip, 0.4f);
        
    }
    void Source2()
    {

        audioSource2.clip = AudioClipArray[Random.Range(0, AudioClipArray.Length)];
        audioSource2.PlayOneShot(audioSource2.clip, 0.4f);
    }
    void Source3()
    {

        audioSource3.clip = AudioClipArray[Random.Range(0, AudioClipArray.Length)];
        audioSource3.PlayOneShot(audioSource3.clip, 0.4f);
    }
    //Stop all Audio no matter where when not in range. Applies for all 3
    void AudioStop()
    {
        audioSource1.Stop();
        audioSource2.Stop();
        audioSource3.Stop();
    }
}

