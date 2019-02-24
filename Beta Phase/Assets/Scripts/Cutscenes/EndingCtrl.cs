using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingCtrl : MonoBehaviour
{
    public CameraLogic cameraL;
    public Transform yingYue, cameraT;
    public Transform[] credits;
    public Vector3 targetOffset;
    public float panSpeed;
    public RPGTalk rpgT;
    public GameObject endTitle, fadeToBlack, fadeFromBlack, newspaper, skipButton;
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
            creditBgm.volume += Time.deltaTime / 2; //fades in music
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
        skipButton.SetActive(true);
        creditBgm.Play();
        cameraT.position = credits[0].position;     //logo
        cameraT.rotation = credits[0].rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits[1].position;     //XP
        cameraT.rotation = credits[1].rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits[2].position;     //Noah
        cameraT.rotation = credits[2].rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits[3].position;     //Cherie
        cameraT.rotation = credits[3].rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits[4].position;     //Marcus
        cameraT.rotation = credits[4].rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits[5].position;     //Skyler
        cameraT.rotation = credits[5].rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits[6].position;     //Harits
        cameraT.rotation = credits[6].rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits[7].position;     //Kaffy
        cameraT.rotation = credits[7].rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits[8].position;     //Voice
        cameraT.rotation = credits[8].rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits[9].position;     //Misc music
        cameraT.rotation = credits[9].rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits[10].position;     //Font
        cameraT.rotation = credits[10].rotation;
        yield return new WaitForSeconds(4);
        cameraT.position = credits[11].position;     //Thanks
        cameraT.rotation = credits[11].rotation;
        yield return new WaitForSeconds(4);
        uiB.LoadScene("FakeMenu");
    }
}
