using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottleSelect : MonoBehaviour {

    public BottleThrow bottleThrow;
    public bool selectedBottle, hoverButton;
    public GameObject targetRing;
    public Image bottleFilled;
    public LayerMask layerMask;
    [SerializeField]
    ItemPickUp pickUp;
    PlayerLogic playerLogic;
    //have something to check if the player has a bottle or not
    private void Start()
    {
        playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic >();
    }
    private void Update()
    {
        if (selectedBottle)
        {
            playerLogic.noMoving = true;
            RaycastTarget();
        }

        if (Input.GetMouseButton(0) && selectedBottle && !hoverButton)
        {
            print("shootbottle");
            bottleThrow.notPlayer = false;
            playerLogic.noMoving = false;
            bottleThrow.enabled = true;
            selectedBottle = false;
            targetRing.SetActive(false);
            pickUp.haveBottle = false;
            bottleFilled.enabled = false;
        }
    }

    void RaycastTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if(hit.transform.tag == "Path")
            {
                //targetRing.transform.position = hit.point;
                targetRing.transform.position = new Vector3(hit.point.x, hit.point.y + 0.25f, hit.point.z);
                targetRing.transform.rotation = Quaternion.Euler(90, 0, 0);
                bottleThrow.target = hit.point;
            }
            if (hit.transform.name == "Plane")
            {
                targetRing.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (hit.transform.name == "Wall")
            {
                targetRing.transform.rotation = Quaternion.Euler(90, 0, 0);
            }
        }
    }

    public void BottleButton()
    {
        if (!selectedBottle && pickUp.haveBottle)
        {
            selectedBottle = true;
            targetRing.SetActive(true);
        }

        else 
        {
            selectedBottle = false;
            targetRing.SetActive(false);
        }
    }

    public void OnPointerEnter()
    {
        hoverButton = true;
    }

    public void OnPointerExit()
    {
        hoverButton = false;
    }
}
