using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObject : MonoBehaviour {

    public GameObject[] toDisappear;
    public Collider[] colliders;
    public bool forColliders;
    // Use this for initialization

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            if (!forColliders)
            {
                foreach (GameObject obj in toDisappear)
                {
                    obj.SetActive(false);
                    this.gameObject.SetActive(false);
                }
            }

            if (forColliders)
            {
                foreach (Collider obj in colliders)
                {
                    obj.enabled = false;
                }
            }
        }
    }
}
