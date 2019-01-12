using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerToMove : MonoBehaviour {

    public Transform movePaths;
    public Vector3 newPos;
    public ArtificialIntelligence ai;
    bool over;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !over)
        {
            movePaths.position = new Vector3(newPos.x, newPos.y, newPos.z);
            //ai.staticOriginalRotation = new Vector3(0, 0, 0);
            //ai.run = true;
            over = true;
            StartCoroutine(StopRunning());
        }
    }

    IEnumerator StopRunning()
    {
        yield return new WaitForSeconds(5f);
        //ai.run = false;
    }
}
