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

    public void SwitchDialogue()
    {
        if (neededObj.activeInHierarchy || PlayerPrefs.HasKey(neededKeyItemName))
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
