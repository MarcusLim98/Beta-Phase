using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAI : MonoBehaviour {

    public GameObject[] aiToSpawn, aiToDespawn;
    public Transform gate1, gate2;
    public Vector3[] moveGates;
    public bool isSpawning;
    // Use this for initialization
    void Start () {
        //print(gate1.position);  
	}
	
	// Update is called once per frame
	void Update () {
        //print(gate1.position);
        //print(gate2.position);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            gate1.position = new Vector3(moveGates[0].x, moveGates[0].y, moveGates[0].z);
            gate2.position = new Vector3(moveGates[1].x, moveGates[1].y, moveGates[1].z);

            foreach (GameObject ais in aiToSpawn)
            {
                ais.SetActive(true);
            }

            foreach (GameObject ais in aiToDespawn)
            {
                ais.SetActive(false);
            }
            //this.gameObject.SetActive(false);
        }
    }
}
