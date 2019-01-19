using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObject : MonoBehaviour {

    public GameObject[] toDisappear;
    // Use this for initialization

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            foreach(GameObject obj in toDisappear)
            {
                obj.SetActive(false);
                this.gameObject.SetActive(false);
            }
        }
    }
}
