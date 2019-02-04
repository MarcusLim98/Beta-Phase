using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressCheck : MonoBehaviour
{
    [SerializeField]
    string progressName;

    void Awake()
    {
        if (PlayerPrefs.GetInt(progressName) == 1 || PlayerPrefs.HasKey(progressName))
        {
            gameObject.SetActive(false);
        }
    }


}
