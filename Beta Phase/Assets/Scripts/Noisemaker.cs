using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Noisemaker : MonoBehaviour {

    public bool active = false, isNoisyFloor;
    public int routeChange;

    public int noiseType; //1 for player last heard, 2 for bottle-made floors, 3 for natural floors, 4 for fixed, 5 for static distractions, 6 for AI that shoot bottles and chasing debris
    bool staticDistraction;
    public float timer;
    public Text pressE;
    public GameObject distractionObj;
    public Rigidbody rb;
    public Transform[] movePaths;
    public Vector3[] newPos;
    public ArtificialIntelligence[] ai;
    private PlayerLogic playerLogic;
    private SphereCollider sphereCol;
    //private ArtificialIntelligence miniBoss;

    private void Start()
    {
        playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();
        if (noiseType == 2)
        {
            active = true;
        }
        if(noiseType == 5)
        {
            //sphereCol = GetComponent<SphereCollider>();
            rb = distractionObj.GetComponent<Rigidbody>();
        }
        //miniBoss = GameObject.Find("ShootsBottleAI").GetComponent<ArtificialIntelligence>();
    }

    private void Update()
    {
        if (active)
        {
            Counter();
        }
        else timer = 0f;
    }

    private void OnTriggerStay(Collider other)
    {    
        if (other.tag == "Player" && (noiseType == 2 ||noiseType ==3) && playerLogic.movingStyle == 1)
        {
            //print("touched");
            active = true;
        }

        if(other.tag == "Player" && noiseType == 5)
        {
            if (!staticDistraction)
            {
                pressE.enabled = true;
            }
            if (staticDistraction)
            {
                pressE.enabled = false;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                staticDistraction = true;
                rb.AddForce(transform.up * 1000f);
                movePaths[0].position = new Vector3(newPos[0].x, newPos[0].y, newPos[0].z);
                movePaths[1].position = new Vector3(newPos[1].x, newPos[1].y, newPos[1].z);
                ai[0].staticOriginalRotation = new Vector3(0, 46, 0);
                ai[1].staticOriginalRotation = new Vector3(0, -191, 0);
                foreach (ArtificialIntelligence ais in ai)
                {
                    ais.run = true;
                }
            }
        }

        if(other.tag == "Thug")
        {
            foreach (ArtificialIntelligence ais in ai)
            {
                ais.run = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "SpotCheck")
        {
            if (this.gameObject.name == "PlayerBottle(Clone)")
            {
                StartCoroutine("Gone");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && noiseType == 5)
        {
            pressE.enabled = false;
        }
    }

    private void OnTriggerLeave(Collider other)
    {

    }

    public void Counter()
    {
        timer = timer + Time.deltaTime;
        if (timer > 1)
        {
            active = false;
        }
    }

    IEnumerator Gone()
    {
        yield return new WaitForSeconds(8f);
        gameObject.SetActive(false);
    }
}
