using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingCtrl : MonoBehaviour
{
    public CameraLogic cameraL;
    public Transform yingYue, cameraT, credits1, credits2, credits3, credits4, credits5, credits6, credits7, credits8;
    public Vector3 targetOffset;
    public float panSpeed;
    public RPGTalk rpgT;
    public GameObject endTitle, fadeToBlack, fadeFromBlack, newspaper;
    public AudioSource originalBgm, creditBgm, altar;
    public UiBehaviour uiB;
    [SerializeField]
    Camera creditCam;
    [SerializeField]
    CutsceneCallbackMaster callback;

    void Start()
    {
        callback.StartCutscene();
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
        PlayerPrefs.DeleteAll();
        originalBgm.Stop();
        altar.Play();
        endTitle.SetActive(true);
        yield return new WaitForSeconds(4);
        fadeToBlack.SetActive(true);
        yield return new WaitForSeconds(2);
        endTitle.SetActive(false);
        creditCam.enabled = true;
        fadeToBlack.SetActive(false);
        fadeFromBlack.SetActive(true);
        creditBgm.Play();
        cameraT.position = credits1.position;
        cameraT.rotation = credits1.rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits2.position;
        cameraT.rotation = credits2.rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits3.position;
        cameraT.rotation = credits3.rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits4.position;
        cameraT.rotation = credits4.rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits5.position;
        cameraT.rotation = credits5.rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits6.position;
        cameraT.rotation = credits6.rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits7.position;
        cameraT.rotation = credits7.rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits8.position;
        cameraT.rotation = credits8.rotation;
        yield return new WaitForSeconds(4);
        uiB.LoadScene("FakeMenu");
    }
}
