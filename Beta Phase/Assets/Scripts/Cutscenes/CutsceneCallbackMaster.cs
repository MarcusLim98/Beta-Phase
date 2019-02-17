using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneCallbackMaster : MonoBehaviour {

    public PlayerLogic playerLogic;
    public CameraLogic cameraLogic;
    public SpawnBehaviour spawnBeha;
    Transform cameraFocus;
    public GameObject[] camFocusObj, contDialogue, spawnObj, tutObj;

    public UiBehaviour ui;
    [SerializeField]
    Text objectiveText;

    [SerializeField]
    GameObject activeDialogue;
    [SerializeField]
    AudioSource objectiveChime;

    DataSaveAndLoad datasl;

    [SerializeField]
    AIBoss bossAi;

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

    public void EndCutscene()
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
        yield break;
    }

    void Tutorial0()
    {
        tutObj[0].SetActive(true);
        StartCutscene();
    }

    void Tutorial1()
    {
        tutObj[1].SetActive(true);
        StartCutscene();
    }

    void Tutorial2()
    {
        tutObj[2].SetActive(true);
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
        //EndCutscene();
        spawnObj[0].SetActive(true);
        spawnObj[1].SetActive(true);
        StartCutscene();
    }

    void Day1DonePapers()
    {
        ui.LoadScene("Scene 1 CShop");
    }

    //Scene 1
    void Day1Afterwork()
    {
        spawnObj[0].SetActive(true);
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
        Tutorial0();
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
        yield break;
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
        yield break;
    }

    void Day2Notice()
    {
        datasl.ObtainItem("Day2PanToThugs", 1);
        ChangeObjective("distract the guards");
        EndCutscene();
    }

    //Scene 4
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
        yield break;
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

    public void FirstDoc()
    {
        bossAi.stopLaoDa = false;
        contDialogue[0].SetActive(true);                    //Lao Da's intro cutscene
        spawnObj[0].SetActive(true);
        datasl.ObtainItem("BossPhase", 2);                  //got first doc, for LD behaviour
    }

    void BossIntro()                                        //for after intro
    {
        datasl.ObtainItem("BossIntro", 1);                  //for if player saves and dies
        datasl.ObtainItem("BossPhase", 3);                  //done intro, for LD behaviour
        bossAi.stopLaoDa = false;                           //lets LD move and shoot
        EndCutscene();
        spawnObj[0].SetActive(false);                       //turns off YY's talksprite for future eavesdrops
        datasl.SaveGame("SpawnHere84");                     //saves game in case player dies from the first shot
        //make LD shoot here
    }

    public void LastDoc()                                   //Lao Da's outro cutscene
    {
        contDialogue[1].SetActive(true);
    }

    void BossOutro()                                        //for after outro
    {
        datasl.ObtainItem("BossOutro", 1);                  //for if player saves and dies
        //all thugs go alert
        datasl.SaveGame("SpawnHere84");                     //saves after the outro
        StartCoroutine(BossExitPan());
        StartCutscene();
    }

    IEnumerator BossExitPan()
    {
        cameraFocus = camFocusObj[7].transform;             //exit
        yield return new WaitForSeconds(2);
        cameraFocus = playerLogic.gameObject.transform;
        EndCutscene();
        yield break;
    }

    void BossEave1()
    {
        StartCoroutine(BossPan1());
        StartCutscene();
    }

    void BossEave2()
    {
        StartCoroutine(BossPan2());
        StartCutscene();
    }

    void BossEave3()
    {
        StartCoroutine(BossPan3());
        StartCutscene();
    }

    IEnumerator BossPan1()                                  //for panning after first eavesdrop
    {
        datasl.ObtainItem("BossEaveNo1", 1);                //for eavesdrop zone, if player saves and dies
        datasl.ObtainItem("BossPhase", 1);                  //done forced eave 1, for LD behaviour
        cameraFocus = camFocusObj[1].transform;             //1st doc
        yield return new WaitForSeconds(2);
        cameraFocus = playerLogic.gameObject.transform;
        EndCutscene();
        bossAi.triggerFirstEvent = true;
        bossAi.stopLaoDa = false;
        yield break;
    }

    IEnumerator BossPan2()                                  //for panning after second eavesdrop
    {
        datasl.ObtainItem("BossEaveNo2", 1);                //for eavesdrop zone, if player saves and dies
        datasl.ObtainItem("BossPhase", 4);                  //done eave 2, for LD behaviour
        cameraFocus = camFocusObj[2].transform;             //2nd doc
        yield return new WaitForSeconds(2);
        StartCoroutine(BossPan21());
        StartCutscene();
        yield break;
    }

    IEnumerator BossPan21()
    {
        cameraFocus = camFocusObj[3].transform;             //3rd doc
        yield return new WaitForSeconds(2);
        cameraFocus = playerLogic.gameObject.transform;
        bossAi.stopLaoDa = false;
        EaveEnd();
        yield break;
    }

    IEnumerator BossPan3()                                  //for panning after third eavesdrop
    {
        datasl.ObtainItem("BossEaveNo3", 1);                //for eavesdrop zone, if player saves and dies
        datasl.ObtainItem("BossPhase", 5);                  //done eave 3, for LD behaviour
        cameraFocus = camFocusObj[4].transform;             //4th doc
        yield return new WaitForSeconds(2);
        StartCoroutine(BossPan31());
        StartCutscene();
        yield break;
    }

    IEnumerator BossPan31()
    {
        cameraFocus = camFocusObj[5].transform;             //5th doc
        yield return new WaitForSeconds(2);
        StartCoroutine(BossPan32());
        StartCutscene();
        yield break;
    }

    IEnumerator BossPan32()
    {
        cameraFocus = camFocusObj[6].transform;             //6th doc
        yield return new WaitForSeconds(2);
        cameraFocus = playerLogic.gameObject.transform;
        bossAi.stopLaoDa = false;
        EaveEnd();
        yield break;
    }

    #endregion

}
