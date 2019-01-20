using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectDocuments : MonoBehaviour
{
    public Text objectives, pressE;
    int i;
    bool gone;

    private void OnTriggerStay(Collider other)
    {
        if(other.name == "Documents" && i <=3)
        {
            pressE.enabled = true;
            if (Input.GetKeyDown(KeyCode.E))
            {
                print("1");
                i += 1;
                objectives.text = "Collect documents" + " " + "(" + i + "/3)";
                pressE.enabled = false;
                other.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        pressE.enabled = false;
    }
}
