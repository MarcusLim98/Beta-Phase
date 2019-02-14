using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSaving : MonoBehaviour
{
    [SerializeField]
    SpawnBehaviour spawnBeha;
    [SerializeField]
    GameObject lightFeedback;
    //bool happened;

    void Awake()
    {
        spawnBeha = GameObject.Find("Player").GetComponent<SpawnBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") /*&& !happened*/)
        {
            spawnBeha.spawnPointName = gameObject.transform.GetChild(0).name;
            spawnBeha.AutoSave();
            lightFeedback.SetActive(true);
            //happened = true;
            print("altar saving");
        }
    }
}
