using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressCheck : MonoBehaviour
{
    [SerializeField]
    string progressName;

    void Start()
    {
        if (PlayerPrefs.GetInt(progressName) >= 1)
        {
            gameObject.SetActive(false);
        }
    }


}
