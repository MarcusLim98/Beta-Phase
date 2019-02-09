using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingCtrl : MonoBehaviour
{
    public CameraLogic cameraL;
    public Transform yingYue;
    public Vector3 targetOffset;
    public float panSpeed;
    public RPGTalk rpgT;
    public GameObject endTitle, fadeToBlack, fadeFromBlack;
    public AudioSource originalBgm, creditBgm, altar;

    void Start()
    {
        StartCoroutine(PanOut());
    }

    private void Update()
    {
        if (creditBgm.isPlaying)
        {
            creditBgm.volume += Time.deltaTime / 2;
        }
    }

    IEnumerator PanOut()
    {
        yield return new WaitForSeconds(4);
        cameraL.movementSpeed = panSpeed;
        cameraL.targetOffset = targetOffset;
        cameraL.player = yingYue;
        yield return new WaitForSeconds(4);
        rpgT.NewTalk();                         //otherwise, text speed causes bugs
    }

    void PreCredits()
    {
        StartCoroutine(RollCredits());
    }

    IEnumerator RollCredits()
    {
        originalBgm.Stop();
        altar.Play();
        endTitle.SetActive(true);
        yield return new WaitForSeconds(4);
        fadeToBlack.SetActive(true);
        //camera changes position
        yield return new WaitForSeconds(2);
        endTitle.SetActive(false);
        fadeToBlack.SetActive(false);
        fadeFromBlack.SetActive(true);
        creditBgm.Play();
    }
}
