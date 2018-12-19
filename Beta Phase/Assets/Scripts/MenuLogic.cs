using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLogic : MonoBehaviour {

    private Camera thisCamera;
    public Transform getHere;
    public float smoothTime;
    public Vector3 lookHere;
    private bool transition;
    // Use this for initialization
    void Start () {
        thisCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            transition = true;
        }

        if (transition)
        {
            transform.position = Vector3.MoveTowards(transform.position, getHere.position, smoothTime * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation = 
                Quaternion.Euler(new Vector3(lookHere.x, lookHere.y, lookHere.z)), smoothTime * Time.deltaTime);
        }
    }

    public void LoadThis(string name)
    {
        SceneManager.LoadScene(name);
    }
}
