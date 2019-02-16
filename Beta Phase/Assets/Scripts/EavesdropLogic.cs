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
    Image convoBarImage, ear, ear2;
    //private CameraLogic cameraLogic;
    

    void Start () {
        playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();
        convoBarImage = convoBar.GetComponent<Image>();
        ear = GameObject.Find("Ears").transform.GetChild(0).gameObject.GetComponent<Image>();
        ear2 = GameObject.Find("Ears").transform.GetChild(1).gameObject.GetComponent<Image>();
        //cameraLogic = GameObject.Find("Main Camera").GetComponent<CameraLogic>();
    }
	
	
	void Update () {

        if (playerLogic.playerEavesdrop == false)
        {
            images[0].enabled = false;
            images[1].enabled = false;
            ear.enabled = false;
            ear2.enabled = false;
        }
        else if (playerLogic.playerEavesdrop == true)
        {
            //images[0].enabled = true;
            //images[1].enabled = true;
            ear.transform.LookAt(Camera.main.transform);
            ear2.transform.LookAt(Camera.main.transform);
            ear.enabled = true;
            ear2.enabled = true;
        }

        //convoBarImage.fillAmount = currentAmount / 100;
        ear.fillAmount = currentAmount / 100;

        if (currentAmount <= 100 && playerLogic.playerEavesdrop == false)
        {
            isInteracted = false;
        }
        else if (currentAmount <= 100 && playerLogic.playerEavesdrop == true)
        {
            isInteracted = true;
        }
        else if (currentAmount >= 100 && playerLogic.playerEavesdrop == false)
        {
            isInteracted = false;
            currentAmount = 0.01f;
            print("stop");
        }
            /*else if(playerLogic.playerEavesdrop == false)
            {
                isInteracted = false;
                currentAmount = 0.01f;
            }*/

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
