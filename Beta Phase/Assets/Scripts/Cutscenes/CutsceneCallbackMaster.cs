﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneCallbackMaster : MonoBehaviour {

    public PlayerLogic playerLogic;
    public CameraLogic cameraLogic;
    public SpawnBehaviour spawnBeha;
    Transform cameraFocus;
    public GameObject[] camFocusObj, contDialogue, spawnObj;

    public UiBehaviour ui;
    [SerializeField]
    Text objectiveText;

    [SerializeField]
    GameObject activeDialogue;
    [SerializeField]
    AudioSource objectiveChime;

    DataSaveAndLoad datasl;

    void Awake()
    {
        datasl = GameObject.Find("DataController").GetComponent<DataSaveAndLoad>();
    }

    void Start()
    {
        cameraFocus = playerLogic.gameObject.transform;
        objectiveChime.clip = (AudioClip)Resources.Load("Objective Update");
    }

    void Update()
    {
        if (activeDialogue.activeInHierarchy)
        {
            StartCutscene(); //because assigning it to a callback before dialogue doesn't work
        }
    }

    #region GENERAL BEHAVIOURS
    public void StartCutscene()
    {
        playerLogic.DisableMovement();
        cameraLogic.target = cameraFocus;
    }

    void EndCutscene()
    {
        playerLogic.EnableMovement();
    }

    public void ChangeObjective(string newObjective)
    {
        objectiveText.text = newObjective;
        spawnBeha.StopCoroutine("NotifTextBehaviour");
        spawnBeha.StartCoroutine("NotifTextBehaviour", "new objective:" + "\n" + newObjective);
        objectiveChime.Play();
    }

    void EaveEnd()
    {
        playerLogic.playerEavesdrop = false;
        EndCutscene();
    }

    IEnumerator BackToYYContCutscene()
    {
        yield return new WaitForSeconds(2);
        cameraFocus = playerLogic.gameObject.transform;
        contDialogue[0].SetActive(true);
        StartCutscene();
    }
    #endregion



    #region LEVEL 1
    void EndPrologue()
    {
        Time.timeScale = 0.25f;
        ui.LoadScene("Scene 0 Police Office");
    }

    //Scene 0
    void Day1Intro()
    {
        ChangeObjective("pass chief hank the papers");
        EndCutscene();
        spawnObj[0].SetActive(true);
        spawnObj[1].SetActive(true);
    }

    void Day1DonePapers()
    {
        ui.LoadScene("Scene 1 CShop");
    }

    //Scene 1
    void Day1Afterwork()
    {
        spawnObj[2].SetActive(true);
        cameraFocus = camFocusObj[0].transform;
        StartCoroutine(BackToYYContCutscene());
        StartCutscene();
    }

    void Day1Suspicion()
    {
        datasl.ObtainItem("Day1Afterwork", 1);
        ChangeObjective("investigate the ruckus");
        EndCutscene();
    }

    void Day1ApproachThugs()
    {
        ChangeObjective("eavesdrop from afar");
        EndCutscene();
        contDialogue[1].SetActive(false);
        contDialogue[2].SetActive(true);
        spawnObj[0].SetActive(true);
        spawnObj[1].SetActive(true);
    }

    void Day1AfterEavesdrop()
    {
        datasl.ObtainItem("Day1AfterEavesdrop", 1);
        cameraFocus = camFocusObj[0].transform;             //ext coffee shop thugs eaves area
        StartCutscene();
        ChangeObjective("find another way into the coffeeshop");
    }

    void Day1EaveComment()
    {
        EaveEnd();
        cameraFocus = playerLogic.gameObject.transform;     //return to YY
        contDialogue[3].SetActive(true);                    //YY comment on ext coffee shop eaves
    }

    void Day1EnterCShop()
    {
        ChangeObjective("investigate the coffeeshop");
    }

    void Day1AfterCShopEavesdrop()
    {
        datasl.ObtainItem("Day1AfterCShopEavesdrop", 1);
        cameraFocus = camFocusObj[1].transform;             //int coffee shop thugs eaves area
        StartCutscene();
        ChangeObjective("exit area through the side door");
    }

    //Scene 2
    void Day1AfterGambleEavesdrop()
    {
        datasl.ObtainItem("Day1AfterGambleEavesdrop", 1);
        cameraFocus = camFocusObj[0].transform;             //gambling thugs eaves area
        StartCutscene();
        ChangeObjective("exit the area");
    }

    void Day1GambleEaveComment()
    {
        EaveEnd();
        cameraFocus = playerLogic.gameObject.transform;     //return to YY
        contDialogue[0].SetActive(true);                    //YY comment on gamble eaves
    }

    void Day1EndPoint()                                      //for end
    {
        StartCoroutine(Scene2EndPoint());
        StartCutscene();
    }

    IEnumerator Scene2EndPoint()                            //pans to end point
    {
        cameraFocus = camFocusObj[1].transform;             //end point
        yield return new WaitForSeconds(2);
        cameraFocus = playerLogic.gameObject.transform;
        EndCutscene();
    }
    #endregion



    #region LEVEL 2

    //Scene 2.5
    void Day2Intro()
    {
        ui.LoadScene("Scene 3 OWHouse");
    }

    //Scene 3
    void Day2AfterWork()
    {
        datasl.ObtainItem("Day2AfterWork", 1);
        EndCutscene();
    }

    void Day2PanToThugs()
    {
        cameraFocus = camFocusObj[0].transform;             //guarding thug area
        StartCoroutine(Scene3EndPoint());
        StartCutscene();
    }

    IEnumerator Scene3EndPoint()
    {
        yield return new WaitForSeconds(2);
        cameraFocus = camFocusObj[1].transform;
        StartCoroutine(BackToYYContCutscene());
        StartCutscene();
    }

    void Day2Notice()
    {
        datasl.ObtainItem("Day2PanToThugs", 1);
        ChangeObjective("distract the guards");
        EndCutscene();
    }

    //Scene 3
    void AfterEaveCall()
    {
        datasl.ObtainItem("AfterEaveCall", 1);
        cameraFocus = camFocusObj[0].transform;             //thug solo call eaves area
        StartCutscene();
    }

    void AfterEaveDocs()
    {
        datasl.ObtainItem("AfterEaveDocs", 1);
        cameraFocus = camFocusObj[1].transform;             //two thugs talking about docs eaves area
        StartCutscene();
    }

    void Day2Spotter()
    {
        datasl.ObtainItem("Day2Spotter", 1);
        cameraFocus = camFocusObj[2].transform;             //spotter area
    }

    void CMWHObjective()
    {
        ChangeObjective("Collect documents (0/3)");         //may not be used
    }
    #endregion



    #region LEVEL 3

    //Scene 4.5
    void Day3Intro()
    {
        spawnObj[0].SetActive(true);                        //temp blackout
        spawnObj[1].SetActive(false);                       //npc sprite
        StartCoroutine(Day3IntroBeha());                    
    }

    IEnumerator Day3IntroBeha()
    {
        yield return new WaitForSeconds(2);
        spawnObj[2].SetActive(true);                        //TalkArea to trigger monolouge
    }

    void Day3Determination()
    {
        ui.LoadScene("Scene 8 ABHouse");
    }

    //Scene 8
    void BossEaveGen()                                      //for all boss eavesdrops dialogue
    {
        cameraFocus = camFocusObj[0].transform;             //Lao Da
        StartCutscene();
    }

    void BossEave1()
    {
        camFocusObj[1].SetActive(true);                     //1st doc

        StartCoroutine(BossPan1());
    }

    void BossEave2()
    {
        camFocusObj[2].SetActive(true);                     //2nd doc
        camFocusObj[3].SetActive(true);                     //3rd doc

        StartCoroutine(BossPan2());
    }

    void BossEave3()
    {
        camFocusObj[4].SetActive(true);                     //4th doc
        camFocusObj[5].SetActive(true);                     //5th doc
        camFocusObj[6].SetActive(true);                     //6th doc

        StartCoroutine(BossPan3());
    }

    IEnumerator BossPan1()                                 //for panning after first eavesdrop
    {
        //datasl.ObtainItem("BossEaveNo", 1);
        cameraFocus = camFocusObj[1].transform;             //1st doc
        yield return new WaitForSeconds(2);
        cameraFocus = playerLogic.gameObject.transform;
        EndCutscene();
    }

    IEnumerator BossPan2()                                 //for panning after second eavesdrop
    {
        //datasl.ObtainItem("BossEaveNo", 2);

        cameraFocus = camFocusObj[2].transform;             //2nd doc
        yield return new WaitForSeconds(2);
        cameraFocus = camFocusObj[3].transform;             //3rd doc
        yield return new WaitForSeconds(2);
        cameraFocus = playerLogic.gameObject.transform;
        EndCutscene();
    }

    IEnumerator BossPan3()                                 //for panning after third eavesdrop
    {
        //datasl.ObtainItem("BossEaveNo", 3);

        cameraFocus = camFocusObj[4].transform;             //4th doc
        yield return new WaitForSeconds(2);
        cameraFocus = camFocusObj[5].transform;             //5th doc
        yield return new WaitForSeconds(2);
        cameraFocus = camFocusObj[6].transform;             //6th doc
        yield return new WaitForSeconds(2);
        cameraFocus = playerLogic.gameObject.transform;
        EndCutscene();
    }

    #endregion

}
