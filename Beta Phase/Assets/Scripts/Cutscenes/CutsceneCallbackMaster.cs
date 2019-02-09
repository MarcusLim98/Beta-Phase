using System.Collections;
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


    DataSaveAndLoad datasl;

    void Awake()
    {
        datasl = GameObject.Find("DataController").GetComponent<DataSaveAndLoad>();
    }

    void Start()
    {
        cameraFocus = playerLogic.gameObject.transform;
    }

    void Update()
    {
        if (activeDialogue.activeInHierarchy)
        {
            StartCutscene(); //because assigning it to a callback before dialogue doesn't work
        }
    }

#region GENERAL BEHAVIOURS
    void StartCutscene()
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

    void Day1Afterwork()
    {
        spawnObj[2].SetActive(true);
        cameraFocus = camFocusObj[0].transform;
        StartCoroutine(BackToYYContCutscene());
        StartCutscene();
    }

    void Day1Suspicion()
    {
        datasl.ObtainItem("Day1Suspicion");
        //PlayerPrefs.SetInt("Day1Suspicion", 1);
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
        datasl.ObtainItem("Day1AfterEavesdrop");
        //PlayerPrefs.SetInt("Day1AfterEavesdrop", 1);
        cameraFocus = camFocusObj[0].transform;
        StartCutscene();
        ChangeObjective("find another way into the coffeeshop");
    }

    void Day1EaveComment()
    {
        EaveEnd();
        cameraFocus = playerLogic.gameObject.transform;
        contDialogue[3].SetActive(true);
    }

    void Day1EnterCShop()
    {
        ChangeObjective("investigate the coffeeshop");
    }

    void Day1AfterCShopEavesdrop()
    {
        datasl.ObtainItem("Day1AfterCShopEavesdrop");
        //PlayerPrefs.SetInt("Day1AfterCShopEavesdrop", 1);
        cameraFocus = camFocusObj[1].transform;
        StartCutscene();
        ChangeObjective("exit area through the side door");
    }

    void Day1AfterGambleEavesdrop()
    {
        datasl.ObtainItem("Day1AfterGambleEavesdrop");
        //PlayerPrefs.SetInt("Day1AfterGambleEavesdrop", 1);
        cameraFocus = camFocusObj[0].transform;
        StartCutscene();
        ChangeObjective("exit the area");
    }

    void Day1GambleEaveComment()
    {
        EaveEnd();
        cameraFocus = playerLogic.gameObject.transform;
        contDialogue[0].SetActive(true);
    }
    #endregion



    #region LEVEL 2
    void Day2Intro()
    {
        ui.LoadScene("Scene 3 OWHouse");
    }

    void Day2AfterWork()
    {
        datasl.ObtainItem("Day2AfterWork");
        //PlayerPrefs.SetInt("Day2AfterWork", 1);
        EndCutscene();
    }

    void Day2PanToThugs()
    {
        cameraFocus = camFocusObj[0].transform;
        StartCoroutine(BackToYYContCutscene());
        StartCutscene();
    }

    void Day2Notice()
    {
        datasl.ObtainItem("Day2PanToThugs");
        //PlayerPrefs.SetInt("Day2Notice", 1);
        ChangeObjective("distract the guards");
        EndCutscene();
    }

    void AfterEaveDocs()
    {
        datasl.ObtainItem("AfterEaveDocs");
        //PlayerPrefs.SetInt("AfterEaveDocs", 1);
        cameraFocus = camFocusObj[0].transform;
        StartCutscene();
    }

    void Day2Spotter()
    {
        datasl.ObtainItem("Day2Spotter");
        //PlayerPrefs.SetInt("Day2Spotter", 1);
        cameraFocus = camFocusObj[1].transform;
    }

    void CMWHObjective()
    {
        ChangeObjective("Collect documents (0/3)");
    }
#endregion



#region LEVEL 3
    void Day3Intro()
    {
        ui.LoadScene("Scene 8 ABHouse");
    }

    void BossEave()
    {
        datasl.ObtainItem("BossEave");
        //PlayerPrefs.SetInt("BossEave", 1);
        cameraFocus = camFocusObj[0].transform;                     //Lao Da
        StartCutscene();
    }

    IEnumerator BossEave1()
    {
        datasl.ObtainItem("BossEave1");
        //PlayerPrefs.SetInt("BossEave1", 1);
        cameraFocus = camFocusObj[1].transform;                     //1st doc
        yield return new WaitForSeconds(2);
        cameraFocus = playerLogic.gameObject.transform;
    }

    IEnumerator BossEave2()
    {
        datasl.ObtainItem("BossEave2");
        //PlayerPrefs.SetInt("BossEave2", 1);
        cameraFocus = camFocusObj[2].transform;                     //2nd doc
        yield return new WaitForSeconds(2);
        cameraFocus = camFocusObj[3].transform;                     //3rd doc
        yield return new WaitForSeconds(2);
        cameraFocus = playerLogic.gameObject.transform;
    }

    IEnumerator BossEave3()
    {
        datasl.ObtainItem("BossEave3");
        //PlayerPrefs.SetInt("BossEave3", 1);
        cameraFocus = camFocusObj[4].transform;                     //4th doc
        yield return new WaitForSeconds(2);
        cameraFocus = camFocusObj[5].transform;                     //5th doc
        yield return new WaitForSeconds(2);
        cameraFocus = playerLogic.gameObject.transform;
    }

    void TheEnd()
    {
        spawnObj[0].SetActive(true);
        //ui.LoadScene("Credits");
    }

    #endregion

}
