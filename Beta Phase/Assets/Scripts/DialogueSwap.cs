using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSwap : MonoBehaviour {

    public RPGTalk rpgTalk;
    public Image yyTextBox, npcTextBox, yyImage, npcImage;
    public Text text, nameOfCharacter;
    public Vector3[] uiSpot;
    public Sprite[] npcs;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (rpgTalk.npcBoxTurn == false)
        //{
        //    yyTextBox.enabled = true;
        //    yyImage.enabled = true;
        //    npcTextBox.enabled = false;
        //    npcImage.enabled = false;
        //    //print(name.transform.localPosition);
        //    nameOfCharacter.transform.localPosition = new Vector3(uiSpot[2].x, uiSpot[2].y, uiSpot[2].z);
        //    text.transform.localPosition = new Vector3(uiSpot[0].x, uiSpot[0].y, uiSpot[0].z);
        //}
        //else if (rpgTalk.npcBoxTurn == true)
        //{
        //    if (rpgTalk.dialogerUI.text == "QQ")
        //    {
        //        npcImage.sprite = npcs[0];
        //    }
        //    else if (rpgTalk.dialogerUI.text == "Hank")
        //    {
        //        npcImage.sprite = npcs[1];
        //    }
        //    yyTextBox.enabled = false;
        //    yyImage.enabled = false;
        //    npcTextBox.enabled = true;
        //    npcImage.enabled = true;
        //    //print(nameOfCharacter.transform.localPosition);
        //    nameOfCharacter.transform.localPosition = new Vector3(uiSpot[3].x, uiSpot[3].y, uiSpot[3].z);
        //    text.transform.localPosition = new Vector3(uiSpot[1].x, uiSpot[1].y, uiSpot[1].z);
        //}
    }
}
