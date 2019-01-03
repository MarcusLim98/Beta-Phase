using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EavesdropLogic : MonoBehaviour
{
    public Image[] images;
    public Transform convoBar;
    public float currentAmount, speed;
    public bool isMouseDown, changeCameraAngle;
    private float downTime;
    private PlayerLogic playerLogic;
    //private CameraLogic cameraLogic;
    

    void Start () {
        playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();
        //cameraLogic = GameObject.Find("Main Camera").GetComponent<CameraLogic>();
    }
	
	
	void Update () {

        if(playerLogic.playerEavesdrop == false)
        {
            images[0].enabled = false;
            images[1].enabled = false;
        }
        else if (playerLogic.playerEavesdrop == true)
        {
            images[0].enabled = true;
            images[1].enabled = true;
        }

        convoBar.GetComponent<Image>().fillAmount = currentAmount / 100;

        if(!isMouseDown && currentAmount >= 0.01f && playerLogic.cursorIsOverUI == true)
        {
            currentAmount -= speed * Time.deltaTime;
        }
        else if (isMouseDown == true && Input.GetMouseButton(0) && currentAmount <= 100 && playerLogic.cursorIsOverUI == true)
        {
            changeCameraAngle = true;
            currentAmount += speed * Time.deltaTime;
        }
        else
        {
            currentAmount = 0.01f;
            changeCameraAngle = false;
        }
        /*else if(isMouseDown == true && Input.GetMouseButtonUp(0) && currentAmount >= 0.1f)
        {
            currentAmount -= speed * Time.deltaTime;
            print("3");
        }*/
    }

    public void Hover()
    {
        isMouseDown = true;
        //print("on");
    }

    public void Exit()
    {
        isMouseDown = false;
        //print("off");
    }
}
