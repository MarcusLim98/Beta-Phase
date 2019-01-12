using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotatingParticleEffect : MonoBehaviour {

    public bool onlyTurn;
    public Vector3 speed;
    public ParticleSystem ps1, ps2, ps3;
    public SphereCollider thisCollider;
    public Transform[] movePaths;
    public Vector3[] newPos;
    //public ArtificialIntelligence[] ai;
    bool thisPlayer, stopRunningCoroutine;
    EavesdropLogic eavesdropLogic;
    public SpawnBehaviour spawnBehaviour;


    void Start () {
        eavesdropLogic = GameObject.Find("ConvoMeter").GetComponent<EavesdropLogic>();
        thisCollider = GetComponent<SphereCollider>();
    }
	

	void Update () {
        if (!onlyTurn)
        {
            if (eavesdropLogic.changeCameraAngle == false && thisCollider.enabled == true && thisPlayer)
            {
                var main = ps1.main;
                main.startLifetime = 2;
                var main2 = ps2.main;
                main2.startLifetime = 0.00001f;
            }
            else if (eavesdropLogic.changeCameraAngle == true && thisCollider.enabled == true && thisPlayer)
            {
                var main = ps1.main;
                main.startLifetime = 0.00001f;
                var main2 = ps2.main;
                main2.startLifetime = 1f;
            }
            else if (eavesdropLogic.changeCameraAngle == false && thisCollider.enabled == false && thisPlayer && !stopRunningCoroutine)
            {
                var main = ps1.main;
                main.startLifetime = 0.00001f;
                var main2 = ps2.main;
                main2.startLifetime = 0.00001f;
                var main3 = ps3.main;
                main3.startLifetime = 5f;
               /* if(this.gameObject.name == "EavesdropZone2")
                {
                    Text objective = GameObject.Find("ObjectiveText").GetComponent<Text>();
                    objective.text = "Exit the warehouse";
                    movePaths[0].position = new Vector3(newPos[0].x, newPos[0].y, newPos[0].z);
                    movePaths[1].position = new Vector3(newPos[1].x, newPos[1].y, newPos[1].z);
                    ai[0].staticOriginalRotation = new Vector3(0, 90, 0);
                    ai[1].staticOriginalRotation = new Vector3(0, 180, 0);
                    foreach (ArtificialIntelligence ais in ai)
                    {
                        ais.run = true;
                    }
                }
                StartCoroutine(Gone());*/
            }
        }
        else if (onlyTurn)
        {
            transform.Rotate(speed * Time.deltaTime);
        }
    }

    IEnumerator Gone()
    {
        print("run");
        spawnBehaviour.StopCoroutine("NotifTextBehaviour");
        spawnBehaviour.StartCoroutine("NotifTextBehaviour", "Eavesdropping completed.");
        stopRunningCoroutine = true;
        yield return new WaitForSeconds(0.5f);
        var main3 = ps3.main;
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
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            thisPlayer = false;
        }
    }
}
