using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportantCutsceneCtrlSwitch : MonoBehaviour {

    public PlayerLogic playerLogic;
    public CameraLogic cameraLogic;
    public Transform cameraFocus;

    void StartCutscene()
    {
        playerLogic.DisableMovement();
    }

    void EndCutscene()
    {
        playerLogic.EnableMovement();
    }

    void SwapCamFocus()
    {
        cameraLogic.target = cameraFocus;
    }

}
