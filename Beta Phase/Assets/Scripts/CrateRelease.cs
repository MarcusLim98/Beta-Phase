using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrateRelease : MonoBehaviour {

    public Text pressE;
    public float fallingForce;
    public GameObject[] crates;
    bool hasInteracted;
    float timer;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (hasInteracted && timer < 0.1f)
        {
            timer += Time.deltaTime;
            foreach (GameObject obj in crates)
            {
                obj.transform.Translate(0, 0, -fallingForce * Time.deltaTime);
                if (timer > 5f)
                {
                    obj.gameObject.SetActive(false);
                }
            }
        }
	}

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                print("hit");
                hasInteracted = true;
                foreach (GameObject obj in crates)
                {
                    obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    //obj.GetComponent<Rigidbody>().AddForce(Vector3.down * fallingForce);
                }
            }
            if (!hasInteracted)
            {
                pressE.text = "Hold E to interact";
                pressE.enabled = true;
            }
            else if (hasInteracted)
            {
                pressE.enabled = false;         
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            pressE.enabled = false;
        }
    }
}
