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
    public bool isInteracted, changeCameraAngle;
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

        if (Input.GetKeyUp(KeyCode.E) && currentAmount <= 100 && playerLogic.playerEavesdrop == true)
        {
            isInteracted = false;
        }
        else if (Input.GetKey(KeyCode.E) && currentAmount <= 100 && playerLogic.playerEavesdrop == true)
        {
            isInteracted = true;
        }
        else if(playerLogic.playerEavesdrop == false)
        {
            currentAmount = 0.01f;
        }

        if (!isInteracted)
        {
            currentAmount -= speed * Time.deltaTime;
            if(currentAmount <= 0.01f)
            {
                currentAmount = 0.01f;
            }
        }
        else if (isInteracted)
        {
            currentAmount += speed * Time.deltaTime;
        }
    }
}
