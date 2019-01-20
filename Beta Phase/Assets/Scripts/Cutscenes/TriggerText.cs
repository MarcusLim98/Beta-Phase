using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerText : MonoBehaviour
{

    public Text tutorialText;
    public bool forObjectives;
    [SerializeField]
    string instructions;
    bool happened;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !happened && !forObjectives)
        {
            TutText();
        }

        if (other.CompareTag("Player") && forObjectives)
        {
            tutorialText.text = instructions;
            tutorialText.GetComponent<RectTransform>().anchoredPosition = new Vector3(17.78333f, -15.83332f, 0);
            gameObject.SetActive(false);
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
        yield return new WaitForSeconds(8);
        tutorialText.text = "";
    }

}
