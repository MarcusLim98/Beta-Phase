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

    void Start()
    {
        StartCoroutine(PanOut());
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
}
