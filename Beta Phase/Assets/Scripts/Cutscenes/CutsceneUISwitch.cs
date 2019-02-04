using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneUISwitch : MonoBehaviour
{
    public Text speakerName;
    public Image[] diaSprites;
    public GameObject[] textBox, screenEdge;
    public GameObject nameObj, diaText, dialogueObj;
    public Vector2[] namePos, textPos, edgePos;

    void Update()
    {
        if (dialogueObj.activeInHierarchy)
        {
            DiaEnter();
        }
        else
        {
            DiaExit();
        }

        if (speakerName.text.Contains("Ying Yue"))
        {
            YingYueTalk();
        }
        else
        {
            NPCTalk();
        }
    }

    void YingYueTalk()
    {
        diaSprites[0].color = Color.Lerp(diaSprites[0].color, Color.white, 1);      //YY sprite turns to white
        diaSprites[1].color = Color.Lerp(diaSprites[1].color, Color.gray, 1);       //NPC sprite turns to gray
        textBox[0].SetActive(true);                                                 //YY text box activated
        textBox[1].SetActive(false);                                                //NPC text box deactivated
        diaText.transform.localPosition = textPos[0];                               //move text to YY's side
        nameObj.transform.localPosition = namePos[0];
    }

    void NPCTalk()
    {
        diaSprites[0].color = Color.Lerp(diaSprites[0].color, Color.gray, 1);       //YY sprite turns to gray
        diaSprites[1].color = Color.Lerp(diaSprites[1].color, Color.white, 1);      //NPC sprite turns to white
        textBox[0].SetActive(false);                                                //YY text box deactivated
        textBox[1].SetActive(true);                                                 //NPC text box activated
        diaText.transform.localPosition = textPos[1];                               //move text to NPC's side
        nameObj.transform.localPosition = namePos[1];
    }


    void DiaEnter()
    {
        screenEdge[0].transform.localPosition = Vector2.MoveTowards(screenEdge[0].transform.localPosition, edgePos[1], 10);
        screenEdge[1].transform.localPosition = Vector2.MoveTowards(screenEdge[1].transform.localPosition, edgePos[3], 10);
    }

    void DiaExit()
    {
        screenEdge[0].transform.localPosition = Vector2.MoveTowards(screenEdge[0].transform.localPosition, edgePos[0], 10);
        screenEdge[1].transform.localPosition = Vector2.MoveTowards(screenEdge[1].transform.localPosition, edgePos[2], 10);
    }

}
