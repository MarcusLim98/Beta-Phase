using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSwitcher : MonoBehaviour {

    [SerializeField]
    GameObject incompleteDialogue, completedDialogue, neededObj;
    [SerializeField]
    string neededKeyItemName;

    void Start() {
        SwitchDialogue();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchDialogue();
        }
    }

    public void SwitchDialogue()
    {
        if (PlayerPrefs.HasKey(neededKeyItemName) || neededObj.activeInHierarchy)
        {
            incompleteDialogue.SetActive(false);
            completedDialogue.SetActive(true);
        }

        else
        {
            incompleteDialogue.SetActive(true);
            completedDialogue.SetActive(false);
        }
    }
}
