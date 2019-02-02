using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoisyFloor : MonoBehaviour
{
    PlayerLogic playerLogic;
    SphereCollider sc;
    // Start is called before the first frame update
    void Start()
    {
        sc = GetComponent<SphereCollider>();
        playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "LosingCondiiton")
        {
            other.GetComponentInParent<ArtificialIntelligence>().noisySource = playerLogic.thisNoisyFloor;
            other.GetComponentInParent<ArtificialIntelligence>().goToNoisySource = true;
            other.GetComponentInParent<ArtificialIntelligence>().questionMark.SetActive(true);
            other.GetComponentInParent<ArtificialIntelligence>().exclamationMark.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "Player" && playerLogic.stepOnNoisyFloor == true)
        {
            sc.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player" && playerLogic.stepOnNoisyFloor == true)
        {
            sc.enabled = false;
        }
    }
}
