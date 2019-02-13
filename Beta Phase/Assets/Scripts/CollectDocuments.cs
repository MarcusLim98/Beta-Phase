using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectDocuments : MonoBehaviour
{
    public Text objectives, pressE;
    public GameObject[] obj;
    //public Vector3 pos;
    public int collect;
    public int i, sceneNo;
    bool gone;
    DataSaveAndLoad dsal;

    private void Start()
    {
        dsal = GameObject.Find("DataController").GetComponent<DataSaveAndLoad>();
    }

    private void Update()
    {
        //print(endPoint.position);
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Documents" && i <=collect)
        {
            pressE.enabled = true;
            if (Input.GetKeyDown(KeyCode.E))
            {
                print("1");
                i += 1;
                objectives.text = "Collect documents" + " " + "(" + i + "/" + collect +")";
                pressE.enabled = false;
                other.gameObject.SetActive(false);
                //if (sceneNo == 6)
                //{
                //    dsal.ObtainItem("WHDocs", i);
                //}
                //else if (sceneNo == 8)
                //{
                //    dsal.ObtainItem("BossDocs", i);
                //}
            }
        }
        if (other.tag == "Documents" && i >= collect)
        {
            foreach(GameObject indicators in obj)
            {
                indicators.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        pressE.enabled = false;
    }

    //private void OnLevelWasLoaded(int level)
    //{
    //    if (level == 6 && PlayerPrefs.HasKey("WHDocs"))
    //    {
    //        i = PlayerPrefs.GetInt("WHDocs");
    //    }

    //    else if (level == 8 && PlayerPrefs.HasKey("BossDocs"))
    //    {
    //        i = PlayerPrefs.GetInt("BossDocs");
    //    }
    //}
}
