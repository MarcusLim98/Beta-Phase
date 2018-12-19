using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotCheck : MonoBehaviour {

    public bool atNoiseSource;
    private ArtificialIntelligence AI;

    private void Start()
    {
        AI= GetComponentInParent<ArtificialIntelligence>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Noisemaker>() || other.name == "YY mesh")
        {
            atNoiseSource = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        atNoiseSource = false;
    }
}
