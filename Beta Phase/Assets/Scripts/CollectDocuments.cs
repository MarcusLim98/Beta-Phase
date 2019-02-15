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
    public int i, lvNo;
    bool gone;
    DataSaveAndLoad dsal;
    [SerializeField]
    AudioSource externalAudio;
    CutsceneCallbackMaster callback;

    private void Start()
    {
        dsal = GameObject.Find("DataController").GetComponent<DataSaveAndLoad>();
        callback = GameObject.Find("CutsceneCallbackMaster").GetComponent<CutsceneCallbackMaster>();

        if (lvNo == 2 && PlayerPrefs.HasKey("WHDocs"))          //if CWHouse and you've taken docs before, load old int
        {
            i = PlayerPrefs.GetInt("WHDocs");
        }

        else if (lvNo == 3 && PlayerPrefs.HasKey("BossDocs"))   //if Boss Level and you've taken docs before, load old int
        { 
            i = PlayerPrefs.GetInt("BossDocs");
        }
    }

    private void Update()
    {
        //print(endPoint.position);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Documents" && i <=collect)
        {
            pressE.text = "Press E to pick up";
            pressE.enabled = true;
            if (Input.GetKeyDown(KeyCode.E))
            {
                print("1");
                i += 1;
                objectives.text = "Collect documents" + " " + "(" + i + "/" + collect +")";
                pressE.enabled = false;
                other.gameObject.SetActive(false);
                externalAudio.Play();
                if (lvNo == 2)
                {
                    dsal.ObtainItem("WHDocs", i);               //updates int, saves as PlayerPref when at another altar
                }
                else if (lvNo == 3)
                {
                    dsal.ObtainItem(other.name, 1);
                    dsal.ObtainItem("BossDocs", i);             //updates int
                    dsal.SaveGame("SpawnHere84");               //saves int immediately afterwards
                    if (i == 1) { callback.FirstDoc(); }
                    else if (i == collect) { callback.LastDoc(); }
                }
            }
        }

        if (other.tag == "Documents" && i >= collect)           //activates end point
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

}
