using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerLogic : MonoBehaviour {

    public float speed;
    public int movingStyle;
    public LayerMask layerMask;
    public Transform thisNoisyFloor;
    [HideInInspector]
    public bool playerEavesdrop, isMoving, noMoving, cursorIsOverUI, enableSpawnCheat, stepOnNoisyFloor;
    public string fileName;
    public SpawnBehaviour[] sb;
    public Transform[] cheatSpawns;
    NavMeshAgent agent;
    Rigidbody rb;
    Animator anim;
    Vector3 clickPosition;
    ArtificialIntelligence AI;
    EavesdropLogic eavesDropLogic;
    CameraLogic cameraLogic;
    AudioSource externalAudio;
    int alterSpots;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        AI = GameObject.FindObjectOfType<ArtificialIntelligence>();
        //eavesDropLogic = GameObject.Find("ConvoMeter").GetComponent<EavesdropLogic>();
        cameraLogic = GameObject.Find("Main Camera").GetComponent<CameraLogic>();
        externalAudio = GetComponent<AudioSource>();
        sb = GameObject.FindObjectsOfType<SpawnBehaviour>();
        StartCoroutine(WaitForNavMesh());
    }

    void Update()
    {
        Raycasting();
        Cheats();
    }

    void Raycasting()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            clickPosition = hit.point;
            if (hit.collider.tag == "Path" && !noMoving && !cursorIsOverUI)
            {
                if (Input.GetMouseButtonDown(0))
                {               
                    agent.speed = 10f;
                    agent.SetDestination(hit.point);
                    movingStyle = 1;
                    fileName = "Run";
                    externalAudio.clip = (AudioClip)Resources.Load(fileName);
                    externalAudio.Play();
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    agent.speed = 3f;
                    agent.SetDestination(hit.point);
                    movingStyle = 2;
                    fileName = "Sneak";
                    externalAudio.clip = (AudioClip)Resources.Load(fileName);
                    externalAudio.Play();
                }
            }
        }

        if (agent.destination == transform.position)
        {
            movingStyle = 0;
            isMoving = false;
            externalAudio.clip = null;
        }
        else if (agent.destination != transform.position && (movingStyle == 1 || movingStyle == 2) && !externalAudio.isPlaying)
        {
            externalAudio.Play();
        }
        else 
        {
            isMoving = true;
        }

        if (movingStyle == 0)
        {
            anim.SetInteger("State", 0);
        }
        else if(movingStyle > 0)
        {
            anim.SetInteger("State", 1);
        }
    }

    void Cheats()
    {
        if (enableSpawnCheat)
        {
            if (Input.GetKey(KeyCode.Alpha2))
            {
                alterSpots = 0;
                StartCoroutine(SpawnThere());
            }
            else if (Input.GetKey(KeyCode.Alpha3))
            {
                alterSpots = 1;
                StartCoroutine(SpawnThere());
            }
            else if (Input.GetKey(KeyCode.Alpha4))
            {
                alterSpots = 2;
                StartCoroutine(SpawnThere());
            }
            else if (Input.GetKey(KeyCode.Alpha5))
            {
                alterSpots = 3;
                StartCoroutine(SpawnThere());
            }
        }
    }

    IEnumerator WaitForNavMesh()
    {
        agent.enabled = false;
        yield return new WaitForSeconds(2f);
        agent.enabled = true;
    }

    IEnumerator SpawnThere()
    {
        agent.enabled = false;
        transform.position = cheatSpawns[alterSpots].position;
        yield return new WaitForSeconds(1f);
        agent.enabled = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "EavesdropZone" && eavesDropLogic.currentAmount <= 100f)
        {
            playerEavesdrop = true;
            if (eavesDropLogic.changeCameraAngle == false)
            {
                cameraLogic.eavesdropLookHere = this.gameObject.transform;
            }
            else if (eavesDropLogic.changeCameraAngle == true)
            {
                cameraLogic.eavesdropLookHere = other.transform.GetChild(0);
            }
        }

        if (other.tag == "EavesdropZone" && eavesDropLogic.currentAmount >= 100f)
        {
            other.GetComponent<RotatingParticleEffect>().thisCollider.enabled = false;
            playerEavesdrop = false;
        }

        if (other.name == "NoisyFloor" && movingStyle == 1)
        {
            stepOnNoisyFloor = true;
            thisNoisyFloor = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "EavesdropZone")
        {
            playerEavesdrop = false;
        }
        if (other.name == "NoisyFloor" && movingStyle == 1)
        {
            stepOnNoisyFloor = false;
        }
    }
}
