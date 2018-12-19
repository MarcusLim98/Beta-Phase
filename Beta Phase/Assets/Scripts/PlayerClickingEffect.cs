using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClickingEffect : MonoBehaviour {

    public GameObject clickEffect;
    public GameObject leftFootDust;
    public ParticleSystem system;
    public float dustIntervals;
    public Vector3[] clickPosition;
    Vector3 whereToGo;
    List<GameObject> clickFeedBack = new List<GameObject>();
    List<GameObject> movingFeedBack = new List<GameObject>();
    PlayerLogic playerLogic;
    // Use this for initialization
    void Start () {
        for (int i = 0; i < 6; i++)
        {
            GameObject objClick = (GameObject)Instantiate(clickEffect);
            clickEffect.SetActive(false);
            clickFeedBack.Add(objClick);
        }
        /*for (int i = 0; i < 12; i++)
        {
            GameObject objDust = (GameObject)Instantiate(leftFootDust);
            objDust.transform.parent = this.transform;
            leftFootDust.SetActive(false);
            movingFeedBack.Add(objDust);
        }*/
        //system = GetComponent<ParticleSystem>();
        playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();
    }

    // Update is called once per frame
    void Update () {
        Raycasting();
        LegDust();
    }

    void LegDust()
    {
        if (playerLogic.isMoving == true)
        {
            dustIntervals += Time.deltaTime;
            if(dustIntervals >= 0.2f)
            {
                dustIntervals = 0;
                //system.Emit(4);
                WalkingFeedBack();
            }
        }
        //else dustIntervals = 0.0f;
    }

    void Raycasting()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, playerLogic.layerMask))
        {
            whereToGo = hit.point;
            if (hit.collider.tag == "Path" && playerLogic.noMoving == false && playerLogic.cursorIsOverUI == false)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    ClickingFeedBack();
                }
            }
        }
    }

   void ClickingFeedBack()
    {
        for (int i = 0; i < clickFeedBack.Count; i++)
        {
            if (!clickFeedBack[i].activeInHierarchy)
            {
                clickFeedBack[i].transform.position = new Vector3(whereToGo.x, whereToGo.y + 0.25f, whereToGo.z);
                clickFeedBack[i].transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                clickFeedBack[i].SetActive(true);
                break;
            }
        }
    }

    void WalkingFeedBack()
    {
        for (int i = 0; i < clickFeedBack.Count; i++)
        {
            if (!movingFeedBack[i].activeInHierarchy)
            {
                movingFeedBack[i].transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                movingFeedBack[i].transform.rotation = Quaternion.Euler(new Vector3(-77f, 0, 0));
                movingFeedBack[i].SetActive(true);
                break;
            }
        }
    }
}
