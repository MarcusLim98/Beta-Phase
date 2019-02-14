using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerLogic : MonoBehaviour {

    public float walkSpeed, runSpeed;
    public int movingStyle;
    public LayerMask layerMask;
    public Transform thisNoisyFloor;
    //[HideInInspector]
    public bool playerEavesdrop, isMoving, noMoving, cursorIsOverUI, enableSpawnCheat, stepOnNoisyFloor, inCutscene;
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
    PauseMenu pauseMenu;
    AudioSource externalAudio;
    int alterSpots;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        AI = GameObject.FindObjectOfType<ArtificialIntelligence>();
        eavesDropLogic = GameObject.Find("ConvoMeter").GetComponent<EavesdropLogic>();
        cameraLogic = GameObject.Find("Main Camera").GetComponent<CameraLogic>();
        pauseMenu = GameObject.Find("Main Camera").GetComponent<PauseMenu>();
        externalAudio = GetComponent<AudioSource>();
        sb = GameObject.FindObjectsOfType<SpawnBehaviour>();
        StartCoroutine(WaitForNavMesh());
    }

    void Update()
    {
        Raycasting();
        Cheats();
        SceneCheat();
    }

    void Raycasting()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            clickPosition = hit.point;
            if (hit.collider.tag == "Path" && !noMoving && !cursorIsOverUI && !inCutscene && pauseMenu.isPaused == false)
            {
                if (Input.GetMouseButtonDown(0))
                {               
                    agent.speed = runSpeed;
                    agent.SetDestination(hit.point);
                    movingStyle = 1;
                    if (!stepOnNoisyFloor)
                    {
                        fileName = "Run";
                    }
                    else if (stepOnNoisyFloor)
                    {
                        fileName = "Wooden Plank run";
                    }
                    externalAudio.clip = (AudioClip)Resources.Load(fileName);
                    externalAudio.Play();
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    agent.speed = walkSpeed;
                    agent.SetDestination(hit.point);
                    movingStyle = 2;
                    if (!stepOnNoisyFloor)
                    {
                        fileName = "Sneak";
                    }
                    else if (stepOnNoisyFloor)
                    {
                        fileName = "Wooden Plank sneak";
                    }
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
        if (pauseMenu.isPaused == false)
        {
            externalAudio.enabled = true;
        }
        else if (pauseMenu.isPaused == true)
        {
            externalAudio.enabled = false;
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

    void SceneCheat()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SceneManager.LoadScene("Scene 0 Police Office");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SceneManager.LoadScene("Scene 1 CShop");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SceneManager.LoadScene("Scene 2 Den");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SceneManager.LoadScene("Scene 3 OWHouse");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SceneManager.LoadScene("Scene 4 CWHouse");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SceneManager.LoadScene("Scene 7 ABHouse");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                SceneManager.LoadScene("Scene 8 ABHouse");
            }
        }
    }

    IEnumerator WaitForNavMesh()
    {
        agent.enabled = false;
        yield return new WaitForSeconds(1);
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
        if(other.name == "LosingCondiiton" || other.name == "LaoDaBullet(Clone)")
        {
            cursorIsOverUI = true;
            anim.SetInteger("State", 0);
            agent.SetDestination(transform.position);
        }

        if (other.tag == "EavesdropZone" && eavesDropLogic.currentAmount <= 100f)
        {
            playerEavesdrop = true;
            cameraLogic.eavesdropLookHere = other.transform.GetChild(0);
            /*if (eavesDropLogic.changeCameraAngle == false)
            {
                cameraLogic.eavesdropLookHere = this.gameObject.transform;
            }
            else if (eavesDropLogic.changeCameraAngle == true)
            {
                cameraLogic.eavesdropLookHere = other.transform.GetChild(0);
            }*/
        }

        if (other.tag == "EavesdropZone" && eavesDropLogic.currentAmount >= 100f)
        {
            other.GetComponent<RotatingParticleEffect>().thisCollider.enabled = false;
            other.GetComponent<RotatingParticleEffect>().StartCoroutine("Gone");
            //playerEavesdrop = false;
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
        if (other.name == "NoisyFloor")
        {
            stepOnNoisyFloor = false;
        }
    }

    public void EnableMovement()
    {
        inCutscene = false;
    }

    public void DisableMovement()
    {
        inCutscene = true;
    }
}
