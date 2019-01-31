﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSaving : MonoBehaviour
{
    [SerializeField]
    SpawnBehaviour spawnBeha;
    bool happened;

    void Awake()
    {
        spawnBeha = GameObject.Find("Player").GetComponent<SpawnBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !happened)
        {
            spawnBeha.spawnPointName = gameObject.transform.GetChild(0).name;
            spawnBeha.AutoSave();
            happened = true;
        }
    }
}
