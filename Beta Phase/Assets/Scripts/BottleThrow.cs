using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleThrow : MonoBehaviour {

    public Vector3 target, toTarget;
    [SerializeField]
    float speed;
    Rigidbody projectileBody;
    MeshRenderer mesh;
    BoxCollider coll;
    public Transform playerBottle, noisyFloor, player, recentEnemyBottle;
    public ArtificialIntelligence[] AI;
    public bool notPlayer;
    public string fileName;
    private AudioClip externalAudio;

    private void Awake()
    {
        projectileBody = gameObject.GetComponent<Rigidbody>();
        mesh = gameObject.GetComponent<MeshRenderer>();
        coll = gameObject.GetComponent<BoxCollider>();

        ResetBottle();
        GetComponent<AudioSource>().clip = (AudioClip)Resources.Load(fileName);
    }

    private void OnEnable()
    {
        transform.position = playerBottle.position;
        projectileBody.isKinematic = false;
        mesh.enabled = true;
        coll.enabled = true;

        if(!notPlayer)
        {
            toTarget = target - transform.position;
            noisyFloor.name = "PlayerBottle";
        }
        else if (notPlayer)
        {
            toTarget = player.position - new Vector3(transform.position.x + Random.Range(1, 2), transform.position.y, transform.position.z);//transform.position;
            noisyFloor.name = "EnemyBottle";
        }

        // Set up the terms we need to solve the quadratic equations.
        float gSquared = Physics.gravity.sqrMagnitude;
        float b = speed * speed + Vector3.Dot(toTarget, Physics.gravity);
        float discriminant = b * b - gSquared * toTarget.sqrMagnitude;


        // Check whether the target is reachable at max speed or less.
        //if (discriminant < 0) {
            // Target is too far away to hit at this speed.
            // Abort, or fire at max speed in its general direction?
        //}

        float discRoot = Mathf.Sqrt(discriminant);

        // Highest shot with the given max speed:
        float T_max = Mathf.Sqrt((b + discRoot) * 2f / gSquared);

        // Most direct shot with the given max speed:
        float T_min = Mathf.Sqrt((b - discRoot) * 2f / gSquared);

        // Lowest-speed arc available:
        float T_lowEnergy = Mathf.Sqrt(Mathf.Sqrt(toTarget.sqrMagnitude * 4f / gSquared));

        float T = T_lowEnergy; // choose T_max, T_min, or some T in-between like T_lowEnergy

        // Convert from time-to-hit to a launch velocity:
        Vector3 velocity = toTarget / T - Physics.gravity * T / 2f;

        // Apply the calculated velocity (do not use force, acceleration, or impulse modes)
        projectileBody.AddForce(velocity, ForceMode.VelocityChange);

    }

    private void OnDisable()
    {
        ResetBottle();
    }

    void ResetBottle()
    {
        projectileBody.isKinematic = true;
        mesh.enabled = false;
        coll.enabled = false;
        //transform.position = playerBottle.position;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            //Instantiate(noisyFloor, transform.position, Quaternion.Euler(90, 0,0));
            //noisyFloor.GetComponent<Rigidbody>().isKinematic = true;
        }
        if (collision.gameObject.tag == "Path")
        {
            GetComponent<AudioSource>().Play();
            notPlayer = false;
            playerBottle = GameObject.Find("Playershoot").transform;
            //Instantiate(noisyFloor, new Vector3(transform.position.x, 0.1f, transform.position.z), Quaternion.identity);
            //gameObject.GetComponent<BottleThrow>().enabled = false;
            Instantiate(noisyFloor, new Vector3(transform.position.x, 0.15f, transform.position.z -1f), Quaternion.Euler(-90, 0 ,0)  );
            gameObject.GetComponent<BottleThrow>().enabled = false;
            /*recentEnemyBottle = GameObject.Find("EnemyBottle(Clone)").transform;
            if(recentEnemyBottle.name == "EnemyBottle(Clone)")
            {
                foreach (ArtificialIntelligence ai in AI)
                {
                    ai.enemyType = 3;
                    ai.noiseTarget = GameObject.Find("EnemyBottle(Clone)").transform;
                }
            }*/
        }
    }
}
