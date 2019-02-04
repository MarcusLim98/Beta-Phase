using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItemDialogue : MonoBehaviour {

    [SerializeField]
    GameObject keyItemObj;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            keyItemObj.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
