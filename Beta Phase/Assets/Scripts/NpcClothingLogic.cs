using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcClothingLogic : MonoBehaviour {

    public GameObject NPC;
    public Material material;
    public Transform[] enemyPos;
    int npcNo;
         public Color altColor = Color.black;
    // Use this for initialization
    void Start () {
        foreach (Transform pos in enemyPos)
        {
            GameObject AI = Instantiate(NPC, pos.position, Quaternion.identity);

            AI.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

            AI.transform.parent = transform;
            AI.name = "NPC" + " " + npcNo;
            npcNo++;
        }

    }

    // Update is called once per frame
    void Update () {
		
	}
}
