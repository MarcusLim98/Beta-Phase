using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrateRelease : MonoBehaviour {

    public Text pressE;
    public float fallingForce;
    public GameObject[] crates;
    public Vector3[] startingPos, startingRot;
    bool hasInteracted;
    float timer;
	// Use this for initialization
	void Start () {
        for (int i = 0; i < crates.Length; i++)
        {
            startingPos[i] = crates[i].transform.position;
            startingRot[i] = new Vector3(crates[i].transform.rotation.x, crates[i].transform.rotation.y, crates[i].transform.rotation.z);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (hasInteracted)
        {
            timer += Time.deltaTime;
            foreach (GameObject obj in crates)
            {
                //obj.transform.Translate(0, 0, -fallingForce * Time.deltaTime);
                if (timer > 12f)
                {
                    for (int i = 0; i < startingPos.Length; i++)
                    {
                        crates[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY
                        | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                        crates[i].transform.position = startingPos[i];
                        crates[i].transform.rotation = Quaternion.Euler(-90, 0, 0);
                    }
                    timer = 0;
                    hasInteracted = false;
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
