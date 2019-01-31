using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalTriggerCallback : MonoBehaviour
{
    [SerializeField]
    MonoBehaviour scriptName;
    [SerializeField]
    string functionName;
    [SerializeField]
    bool onlyOnce;
    bool happened;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            scriptName.StartCoroutine(functionName);
            if (onlyOnce && !happened)
            {
                happened = true;
            }
        }
    }
}
