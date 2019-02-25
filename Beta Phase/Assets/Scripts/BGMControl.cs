using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMControl : MonoBehaviour
{
    public AudioSource Bgm, Chase;
    public GameObject Spotlight, darkUI;
    string Office, Tutorial, Den, Warehouse, Chasing, Boss , Loss;
    ArtificialIntelligence ChaseCheck;

    // Start is called before the first frame update
    void Start()
    {
        Tutorial = "CoffeeShop";
        Den = "Den and Outisde";
        Warehouse = "Warehouse 1";
        Chasing = "Danger Chase";
        Boss = "Boss Level";
        Loss = "Loss";
        Spotlight.SetActive(false);
        darkUI.SetActive(false);

        SceneCheck();

        Bgm.volume = 0.2f;
        Chase.volume = 0.2f;

    }

    // Update is called once per frame
    void Update()
    {
        //only to test in BGMTest scene
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    ChangeDanger();
        //}
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    EscapeDanger();
        //}
    }

    void SceneCheck()
    {
        //finds scene name and loads approriate bgm
        Scene CurrentScene = SceneManager.GetActiveScene();

        //if (CurrentScene.name =="Scene 0 Police Office" || CurrentScene.name == "Scene 2.5 Police Office " || CurrentScene.name == "Scene 4.5 Police Office ")
        //{
        //    Bgm.clip = (AudioClip)Resources.Load(Office);
        //    Bgm.Play();
        //}

        if (CurrentScene.name == "Scene 1 CShop")
        {
            Bgm.clip = (AudioClip)Resources.Load(Tutorial);
            Bgm.Play();
        }

        if (CurrentScene.name == "Scene 2 Den")
        {
            Bgm.clip = (AudioClip)Resources.Load(Den);
            Bgm.Play();
        }
        if (CurrentScene.name == "Scene 3 OWHouse")
        {
            Bgm.clip = (AudioClip)Resources.Load(Den);
            Bgm.Play();
        }
        if (CurrentScene.name == "Scene 4 CWHouse")
        {
            Bgm.clip = (AudioClip)Resources.Load(Warehouse);
            Bgm.Play();
        }
        if (CurrentScene.name == "Scene 8 ABHouse")
        {
            Bgm.clip = (AudioClip)Resources.Load(Boss);
            Bgm.Play();
        }
        if (CurrentScene.name == "BGMTest")
        {
            Bgm.clip = (AudioClip)Resources.Load(Den);
            Bgm.Play();
        }

    }
    //call this function when AI spots/chases YY
    public void ChangeDanger()
    {
        Chase.clip = (AudioClip)Resources.Load(Chasing);
        if (!Chase.isPlaying)
        {
            Chase.Play();
            Spotlight.SetActive(true);
            darkUI.SetActive(true);
        }

        Bgm.Stop();
    }
    //call this function when YY escapes AI
    public void EscapeDanger()
    {
        StartCoroutine(FadeOut(Chase ,1.5f));
        if (!Bgm.isPlaying)
        {
            Bgm.Play();
        }
        Spotlight.SetActive(false);
        darkUI.SetActive(false);
    }

    public void GotCaught()
    {
        StartCoroutine(FadeOut(Chase, 0.5f));
        Bgm.clip = (AudioClip)Resources.Load(Loss);
        Bgm.Play();
    }
    //Chase volume will decrease before stopping. FadeOut
    IEnumerator FadeOut(AudioSource Chase, float FadeTime)
    {
        float startVolume = Chase.volume;

        while (Chase.volume > 0)
        {
            Chase.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        Chase.Stop();
        Chase.volume = startVolume;
    }
}
