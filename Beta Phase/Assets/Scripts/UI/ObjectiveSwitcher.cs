using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveSwitcher : MonoBehaviour {

    [SerializeField]
    Text objectiveText;

    public void ChangeObjective(string newObjective)
    {
        objectiveText.text = newObjective;
    }
}
