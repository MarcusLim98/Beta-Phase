using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneCallbackMaster : MonoBehaviour {

    public PlayerLogic playerLogic;
    public CameraLogic cameraLogic;
    Transform cameraFocus;
    public GameObject[] camFocusObj, contDialogue, spawnObj;

    public UiBehaviour ui;
    public GameObject DiaCheck;
    [SerializeField]
    Text objectiveText;

    [SerializeField]
    GameObject activeDialogue;

    void Start()
    {
        cameraFocus = playerLogic.gameObject.transform;
    }

    void Update()
    {
        if (activeDialogue.activeInHierarchy)
        {
            StartCutscene();
        }
        
    }

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
    }



    void EndPrologue()
    {
        Time.timeScale = 0.25f;
        ui.LoadScene("police office");
    }

    void Day1Intro()
    {
        ChangeObjective("pass chief hank the papers");
        EndCutscene();
    }

    void Day1DonePapers()
    {
        ui.LoadScene("Scene 1 CShop");
    }

    void Day1Afterwork()
    {
        cameraFocus = camFocusObj[0].transform;
        StartCoroutine(BackToYYContCutscene());
        StartCutscene();
    }

    IEnumerator BackToYYContCutscene()
    {
        yield return new WaitForSeconds(2);
        cameraFocus = playerLogic.gameObject.transform;
        contDialogue[0].SetActive(true);
    }

    void Day1Suspicion()
    {
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
        ChangeObjective("find another way into the coffeeshop");
    }

    void Day1EnterCShop()
    {
        ChangeObjective("investigate the coffeeshop");
    }

    void Day1AfterCShopEacesdrop()
    {
        ChangeObjective("exit area through the side door");
    }

    void Day2Spotter()
    {
        cameraFocus = camFocusObj[1].transform;
        if (DiaCheck.activeInHierarchy == false)
        {
            StartCoroutine(BackToYYafterSpot());
            StartCutscene();
        }
    }

    IEnumerator BackToYYafterSpot()
    {
        yield return new WaitForSeconds(1);
        cameraFocus = playerLogic.gameObject.transform;
        contDialogue[0].SetActive(true);
    }




}
