using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Noisemaker : MonoBehaviour {

    //public bool active = false, isNoisyFloor;
    //public int routeChange;

    //public int noiseType; //1 for player last heard, 2 for bottle-made floors, 3 for natural floors, 4 for fixed, 5 for static distractions, 6 for AI that shoot bottles and chasing debris
    //bool staticDistraction;
    //public float timer;
    public Text pressE;
    //public GameObject distractionObj;
    public Rigidbody rb;
    //public Transform[] movePaths;

    public ArtificialIntelligence[] ai;
    public Vector3[] movePaths;
    public bool hasInteracted;
    //private PlayerLogic playerLogic;
    //private SphereCollider sphereCol;
    //private ArtificialIntelligence miniBoss;

    private void Start()
    {
        /*playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();
        if (noiseType == 2)
        {
            active = true;
        }
        if(noiseType == 5)
        {
            //sphereCol = GetComponent<SphereCollider>();
            rb = distractionObj.GetComponent<Rigidbody>();
        }*/
        //miniBoss = GameObject.Find("ShootsBottleAI").GetComponent<ArtificialIntelligence>();

    }

    private void Update()
    {
        /*if (active)
        {
            Counter();
        }
        else timer = 0f;*/
    }

    private void OnTriggerStay(Collider other)
    {
        /*if (other.tag == "Player" && (noiseType == 2 ||noiseType ==3) && playerLogic.movingStyle == 1)
        {
            //print("touched");
            active = true;
        }*/

        if (other.tag == "Player" && !hasInteracted)
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

        /*private void OnTriggerEnter(Collider other)
        {
            if (other.name == "SpotCheck")
            {
                if (this.gameObject.name == "PlayerBottle(Clone)")
                {
                    StartCoroutine("Gone");
                }
            }
        }*/

        /*private void OnTriggerLeave(Collider other)
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
        }*/
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && !hasInteracted)
        {
            pressE.enabled = false;
        }
    }
}
