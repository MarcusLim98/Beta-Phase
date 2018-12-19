using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickEffectTimeOut : MonoBehaviour {

    public float disappear;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnEnable()
    {
        Invoke("HideObject", disappear);
    }

    void HideObject()
    {
        gameObject.SetActive(false);
    }
}
