using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerText : MonoBehaviour
{

    public Text tutorialText;
    [SerializeField]
    string instructions;
    bool happened;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !happened)
        {
            TutText();
            happened = true;
        }
    }

    public void TutText()
    {
        StopCoroutine("TutTextBehaviour");
        StartCoroutine("TutTextBehaviour", instructions);
        happened = true;
    }

    IEnumerator TutTextBehaviour(string instructions)
    {
        tutorialText.text = instructions;
        yield return new WaitForSeconds(6);
        tutorialText.text = "";
    }

}
