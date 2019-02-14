using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingCtrl : MonoBehaviour
{
    public CameraLogic cameraL;
    public Transform yingYue;           // , cameraT, credits1, credits2, credits3, credits4, credits5, credits6, credits7, credits8;
    public Vector3 targetOffset;
    public float panSpeed;
    public RPGTalk rpgT;
    public GameObject endTitle, fadeToBlack, fadeFromBlack, newspaper, tempCredits;
    public AudioSource originalBgm, creditBgm, altar;
    public UiBehaviour uiB;

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
        yield return new WaitForSeconds(1);
        newspaper.SetActive(true);
        yield return new WaitForSeconds(6);
        cameraL.movementSpeed = panSpeed;
        cameraL.targetOffset = targetOffset;
        cameraL.player = yingYue;
        yield return new WaitForSeconds(4);
        rpgT.NewTalk();                             //otherwise, text speed causes bugs
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
        //camera changes position to credits 1
        yield return new WaitForSeconds(2);
        endTitle.SetActive(false);
        tempCredits.SetActive(true);                //temp until proper credits are used
        fadeToBlack.SetActive(false);
        fadeFromBlack.SetActive(true);
        creditBgm.Play();
        //camera should start panning
        yield return new WaitForSeconds(8);         //change to fit camera pan duration
        uiB.LoadScene("FakeMenu");
    }
}
