using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class CameraLogic : MonoBehaviour {

    public LayerMask layerMask;
    public Transform target;
    public Transform player;
    public Transform eavesdropLookHere;
    public Vector3 targetOffset, raycastPosition;
    public float movementSpeed, length;
    public Outline playerOutlineEffect;
    //public float zoomSpeed, minZoom, maxZoom, zoom;
    bool stopZooming;
    private Camera thisCamera;
    private PlayerLogic playerLogic;
    private EavesdropLogic eavesDropLogic;
    Transform roof, lights;


    void Start () {
        thisCamera = GetComponent<Camera>();
        playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();
        //eavesDropLogic = GameObject.Find("ConvoMeter").GetComponent<EavesdropLogic>();
        target = player.transform;
    }
	

	void Update () {
        Movement();
        Outline();
        CameraAngles();
        //zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        //zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
    }

    void Movement()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + targetOffset, movementSpeed * Time.deltaTime);
    }

    void Outline()
    {
        Ray ray = thisCamera.ViewportPointToRay(new Vector3(raycastPosition.x, raycastPosition.y, raycastPosition.z));
        RaycastHit hit;
        //Debug.DrawRay(ray.origin, ray.direction * length, Color.blue);
        if (Physics.Raycast(ray, out hit, length, layerMask) && playerLogic.isMoving == false)
        {
            playerOutlineEffect.enabled = true;
        }
        else playerOutlineEffect.enabled = false;
    }

    void CameraAngles()
    {
        if (playerLogic.playerEavesdrop == true && !playerLogic.inCutscene && Input.GetKey(KeyCode.E))
        {
            target = eavesdropLookHere.transform;
        }
        else if (!playerLogic.inCutscene)
        {
            target = player.transform;
        }
    }
}
