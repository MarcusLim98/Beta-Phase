using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Noisemaker : MonoBehaviour {

    public Text pressE;
    public Rigidbody rb;

    public ArtificialIntelligence[] ai;
    public Vector3[] movePaths;
    public Vector3[] whereToLook;
    public bool hasInteracted, mustInteract;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !mustInteract && !hasInteracted)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                hasInteracted = true;
                rb.AddForce(transform.up * 1000f);
                for (int i = 0; i < movePaths.Length; i++)
                {
                    ai[i].stationeryPosition.position = movePaths[i];
                    ai[i].questionMark.SetActive(true);
                }
                for (int i = 0; i < whereToLook.Length; i++)
                {
                    ai[i].lookHereStart = whereToLook[i];
                }
            }
            if (!hasInteracted)
            {
                pressE.enabled = true;
            }
            else if (hasInteracted)
            {
                pressE.enabled = false;
                this.gameObject.SetActive(false);
            }
        }

        if (other.tag == "Player" && mustInteract)
        {
            for (int i = 0; i < movePaths.Length; i++)
            {
                ai[i].stationeryPosition.position = movePaths[i];
                ai[i].questionMark.SetActive(true);
            }
            for (int i = 0; i < whereToLook.Length; i++)
            {
                ai[i].lookHereStart = whereToLook[i];
            }
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && !hasInteracted)
        {
            pressE.enabled = false;
        }
    }
}
