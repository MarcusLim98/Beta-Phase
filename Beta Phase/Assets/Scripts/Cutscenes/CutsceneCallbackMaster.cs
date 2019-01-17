using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneCallbackMaster : MonoBehaviour {

    public PlayerLogic playerLogic;
    public CameraLogic cameraLogic;
    Transform cameraFocus;
    public GameObject[] camFocusObj, contDialogue;

    public UiBehaviour ui;
    [SerializeField]
    Text objectiveText;

    [SerializeField]
    GameObject activeDialogue;

    void Start()
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

    IEnumerator BackToYY()
    {
        yield return new WaitForSeconds(2);
        cameraFocus = camFocusObj[0].transform;
    }



    void EndPrologue()
    {
        Time.timeScale = 0.25f;
        ui.LoadScene("Outside_Warehouse");
    }

    void Day1Intro()
    {
        ChangeObjective("pass chief hank the papers");
        EndCutscene();
    }

    void Day1DonePapers()
    {
        ui.LoadScene("");
    }

    void Day1Afterwork()
    {
        cameraFocus = camFocusObj[1].transform;
        StartCoroutine(BackToYY());
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
    }

}
