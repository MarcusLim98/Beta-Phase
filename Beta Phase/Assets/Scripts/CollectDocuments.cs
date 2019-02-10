using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectDocuments : MonoBehaviour
{
    public Text objectives, pressE;
    public GameObject[] obj;
    //public Vector3 pos;
    public int collect, howMany;
    int i;
    bool gone;

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
                objectives.text = "Collect documents" + " " + "(" + i + "/" + howMany +")";
                pressE.enabled = false;
                other.gameObject.SetActive(false);
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
}
