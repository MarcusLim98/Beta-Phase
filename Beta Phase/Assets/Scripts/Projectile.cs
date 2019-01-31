using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public Transform shards;
    public MeshRenderer shardsMesh;
    public Collider shardCollider;
    public bool createdNoise;
    MeshRenderer thisMesh;
    Rigidbody rb;
    float timeToDisappear;
    bool disappear;
    AudioSource externalAudio;
    // Use this for initialization
    void Start()
    {
        thisMesh = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        externalAudio = GetComponent<AudioSource>();
        externalAudio.clip = (AudioClip)Resources.Load("Bottle Shatter");
    }

    // Update is called once per frame
    void Update()
    {
        if (disappear)
        {
            timeToDisappear += Time.deltaTime;
            if (timeToDisappear > 10f)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Path")
        {
            rb.isKinematic = true;
            thisMesh.enabled = false;
            createdNoise = true;
            shardsMesh.enabled = true;
            shardCollider.enabled = true;
            //shards.transform.parent = null;
            shards.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
            shards.transform.rotation = Quaternion.Euler(-90, 0, 0);
            externalAudio.Play();
        }

        if (other.tag == "Thug")
        {
            timeToDisappear = 0;
            disappear = true;
        }
    }
}
