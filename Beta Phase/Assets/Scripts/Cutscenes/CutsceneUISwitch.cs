using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPGTALK.Helper;

public class CutsceneUISwitch : MonoBehaviour
{
    public Image npcSpeaker;
    public GameObject[] textBox, screenEdge;
    public Vector2[] namePos, textPos, edgePos;
    public RPGTalkPhoto[] npcSprites;
    public RPGTalk rpgT;


    void Update()
    {
        if (rpgT.isPlaying)
        {
            DiaEnter();
            if (rpgT.dialogerUI.text.Contains("Ying Yue"))
            {
                YingYueTalk();
            }
            else
            {
                NPCTalk();
            }
        }
        else
        {
            DiaExit();
        }
    }

    void YingYueTalk()
    {
        rpgT.UIPhoto.color = Color.Lerp(rpgT.UIPhoto.color, Color.white, 5 * Time.deltaTime);   //YY sprite turns to white
        npcSpeaker.color = Color.Lerp(npcSpeaker.color, Color.gray, 5 * Time.deltaTime);        //NPC sprite turns to gray
        textBox[0].SetActive(true);                                                             //YY text box activated
        textBox[1].SetActive(false);                                                            //NPC text box deactivated
        rpgT.textUI.transform.localPosition = textPos[0];                                       //move text to YY's side
        rpgT.dialogerUI.transform.localPosition = namePos[0];                                   //move name text to YY's side
    }

    void NPCTalk()
    {
        rpgT.UIPhoto.color = Color.Lerp(rpgT.UIPhoto.color, Color.gray, 5 * Time.deltaTime);    //YY sprite turns to gray
        npcSpeaker.color = Color.Lerp(npcSpeaker.color, Color.white, 5 * Time.deltaTime);       //NPC sprite turns to white
        textBox[0].SetActive(false);                                                            //YY text box deactivated
        textBox[1].SetActive(true);                                                             //NPC text box activated
        rpgT.textUI.transform.localPosition = textPos[1];                                       //move text to NPC's side
        rpgT.dialogerUI.transform.localPosition = namePos[1];                                   //move name text to NPC's side

        for (int i = 0; i < npcSprites.Length; i++)                                             //CHANGES NPC SPRITES
        {
            if (rpgT.npcRef != null && npcSprites[i].name == rpgT.npcRef.originalSpeakerName || npcSprites[i].name == rpgT.rpgtalkElements[0].originalSpeakerName)
            {
                if (npcSpeaker)
                {
                    npcSpeaker.sprite = npcSprites[i].photo;
                    npcSpeaker.SetNativeSize();
                }
            }
        }
    }


    void DiaEnter()                                                                             //moves edges in
    {
        screenEdge[0].transform.localPosition = Vector2.MoveTowards(screenEdge[0].transform.localPosition, edgePos[1], 10);
        screenEdge[1].transform.localPosition = Vector2.MoveTowards(screenEdge[1].transform.localPosition, edgePos[3], 10);
    }

    void DiaExit()                                                                              //moves edges out
    {
        screenEdge[0].transform.localPosition = Vector2.MoveTowards(screenEdge[0].transform.localPosition, edgePos[0], 10);
        screenEdge[1].transform.localPosition = Vector2.MoveTowards(screenEdge[1].transform.localPosition, edgePos[2], 10);
    }

}
