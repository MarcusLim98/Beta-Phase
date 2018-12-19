using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class CameraLogic : MonoBehaviour {

    public LayerMask layerMask;
    private Transform target;
    public Transform player;
    public Transform eavesdropLookHere;
    public Vector3 targetOffset, raycastPosition;
    public float movementSpeed;
    public Outline playerOutlineEffect;
    //public float zoomSpeed, minZoom, maxZoom, zoom;
    bool stopZooming;
    private Camera thisCamera;
    private PlayerLogic playerLogic;
    private EavesdropLogic eavesDropLogic;
    // Use this for initialization
    void Start () {
        thisCamera = GetComponent<Camera>();
        playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();
        eavesDropLogic = GameObject.Find("ConvoMeter").GetComponent<EavesdropLogic>();
        target = player.transform;
    }
	
	// Update is called once per frame
	void Update () {
        Movement();
        Outline();
        CameraAngles();
        //zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        //zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
    }

    void LateUpdate()
    {
        //transform.position = player.position - targetOffset * zoom;
        //transform.LookAt(player.position + Vector3.up * 0);
    }

    void Movement()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + targetOffset, movementSpeed * Time.deltaTime);
    }

    void Outline()
    {
        Ray ray = thisCamera.ViewportPointToRay(new Vector3(raycastPosition.x, raycastPosition.y, raycastPosition.z));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, layerMask) && playerLogic.isMoving == false)
        {
            playerOutlineEffect.enabled = true;
            Debug.DrawRay(ray.origin, ray.direction * 20, Color.blue);
        }
        else playerOutlineEffect.enabled = false;
    }

    void CameraAngles()
    {
        if(playerLogic.playerEavesdrop == false)
        {
            target = player.transform;
        }
        else if (playerLogic.playerEavesdrop == true)
        {
            target = eavesdropLookHere.transform;
        }
    }
}
