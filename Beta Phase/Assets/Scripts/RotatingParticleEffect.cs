using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotatingParticleEffect : MonoBehaviour {

    public bool makeObjDisappear, makeObjAppear;
    public bool onlyTurn, makeAIMove;
    public Text pressE;
    public Vector3 speed;
    //public ParticleSystem ps1, ps2, ps3;
    public SphereCollider thisCollider;
    public GameObject objBeGone, eaveDialogue;
    //public Transform[] movePaths;
    public Vector3[] newPos;
    GameObject particle;
    public ArtificialIntelligence[] ai;
    public Color color1, color2, currentColor;
    bool thisPlayer, stopRunningCoroutine;
    EavesdropLogic eavesdropLogic;
    [SerializeField]
    MonoBehaviour callbackScript;
    [SerializeField]
    string callbackFunction;
    ParticleSystem.MainModule mainModule;
    ParticleSystem ps;
    PlayerLogic playerLogic;
    float startTime;


    void Start () {
        eavesdropLogic = GameObject.Find("ConvoMeter").GetComponent<EavesdropLogic>();
        thisCollider = GetComponent<SphereCollider>();
        playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();
        ps = gameObject.transform.GetChild(1).GetComponent<ParticleSystem>();
        mainModule = ps.main;
        mainModule.startColor = color1;
        currentColor = color1;
        startTime = Time.time;
    }
	

	void Update () {
        if (!onlyTurn)
        {
            /*if (eavesdropLogic.isInteracted == false && thisCollider.enabled == true && thisPlayer)
            {
                var main = ps1.main;
                main.startLifetime = 2;
                var main2 = ps2.main;
                main2.startLifetime = 0.00001f;
                main2.startSize = 0.0f;
            }
            else if (eavesdropLogic.isInteracted == true && thisCollider.enabled == true && thisPlayer)
            {
                var main = ps1.main;
                main.startLifetime = 0.00001f;
                var main2 = ps2.main;
                main2.startLifetime = 1f;
                main2.startSize = 0.25f;
            }
            else */if (thisCollider.enabled == false)
            {
                /*var main = ps1.main;
                main.startLifetime = 0.00001f;
                var main2 = ps2.main;
                main2.startLifetime = 0.00001f;
                main2.startSize = 0.0f;
                var main3 = ps3.main;
                main3.startLifetime = 5f;

                if(makeAIMove)
                {
                    for (int i = 0; i < newPos.Length; i++)
                    {
                        ai[i].stationeryPosition.position = newPos[i];
                    }
                }*/
                //if (this.gameObject.name == "EavesdropZone2")
                //{
                //    Text objective = GameObject.Find("ObjectiveText").GetComponent<Text>();
                //    objective.text = "Exit the warehouse";
                //    movePaths[0].position = new Vector3(newPos[0].x, newPos[0].y, newPos[0].z);
                //    movePaths[1].position = new Vector3(newPos[1].x, newPos[1].y, newPos[1].z);
                //    ai[0].staticOriginalRotation = new Vector3(0, 90, 0);
                //    ai[1].staticOriginalRotation = new Vector3(0, 180, 0);
                //    foreach (ArtificialIntelligence ais in ai)
                //    {
                //        ais.run = true;
                //    }
                //}
                //StartCoroutine(Gone());
                if (makeObjDisappear)
                {
                    objBeGone.SetActive(false);
                }
                else if (makeObjAppear == true)
                {
                    objBeGone.SetActive(true);
                }
            }
        }
        else if (onlyTurn)
        {
            transform.Rotate(speed * Time.deltaTime);
        }

        if(playerLogic.playerEavesdrop == false)
        {
            currentColor = color1;
        }
        else if (playerLogic.playerEavesdrop == true)
        {
            currentColor = color2;  
        }
        mainModule.startColor = Color.Lerp(mainModule.startColor.color, currentColor, 0.1f);
    }

    IEnumerator Gone()
    {
        if (callbackScript != null)     //temp for other scenes to function
        {
            callbackScript.StartCoroutine(callbackFunction);
        }
        if (eaveDialogue != null)       //temp for other scenes to function
        {
            eaveDialogue.SetActive(true);
        }
        print("run");
        stopRunningCoroutine = true;
        yield return new WaitForSeconds(0.5f);
        /*var main3 = ps3.main;
        main3.startLifetime = 0.00001f;
        yield return new WaitForSeconds(2f);
        /*foreach (ArtificialIntelligence ais in ai)
        {
            ais.run = false;
        }*/
        gameObject.SetActive(false);

    }

    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            thisPlayer = true;
            /*if (eavesdropLogic.isInteracted == true)
            {
                pressE.enabled = false;
            }
            else if (eavesdropLogic.isInteracted == false)
            {
                pressE.text = "Hold E to eavesdrop";
                pressE.enabled = true;
            }*/
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            thisPlayer = false;
            //pressE.enabled = false;
        }
    }
}
