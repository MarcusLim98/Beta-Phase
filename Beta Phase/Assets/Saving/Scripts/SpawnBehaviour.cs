using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpawnBehaviour : MonoBehaviour {

    DataSaveAndLoad datasl;
    Text notifText;
    string spawnPointName;
    public GameObject saveButtons;

    void Awake()
    {
        datasl = GameObject.Find("GameController").GetComponent<DataSaveAndLoad>();
    }

    private void Start()
    {
        //notifText = GameObject.Find("NotifText").GetComponent<Text>();
        //saveButtons.SetActive(false);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "SpawnPoint" && Input.GetKeyDown(KeyCode.E))
        {
            //SaveChoiceText();
            print("save now");
            spawnPointName = other.transform.GetChild(0).name;
            print(spawnPointName);
            SaveChoiceText();
        }

        if (other.tag == "KeyItem" && Input.GetKeyDown(KeyCode.E))
        {
            if (PlayerPrefs.GetInt(other.name) == 0 || !PlayerPrefs.HasKey(other.name))
            {
                foreach (KeyItem item in DataSaveAndLoad.keyItemList)
                {
                    if (item.keyItemName == other.name)
                    {
                        if (item.taken == 0)
                        {
                            print(item.taken);
                            StopCoroutine("NotifTextBehaviour");
                            StartCoroutine("NotifTextBehaviour", other.name + " obtained");
                            datasl.ObtainItem(other.name);
                        }
                        else if (item.taken == 1) { print("help"); StopCoroutine("NotifTextBehaviour"); StartCoroutine("NotifTextBehaviour", "Nothing here, huh."); }
                    }
                }
            }
        }
    }

    IEnumerator NotifTextBehaviour(string notif)
    {
        notifText.text = notif;
        yield return new WaitForSeconds(3);
        notifText.text = "";
    }

    void SaveChoiceText()
    {
        //StopCoroutine("NotifTextBehaviour");
        //notifText.text = "It's an altar for worship. Save progress?";
        //saveButtons.SetActive(true);
        print("it is saving");
        datasl.SaveGame(spawnPointName);
    }

    public void ConfirmSave()
    {
        print("Saved!");
        StopCoroutine("NotifTextBehaviour");
        StartCoroutine("NotifTextBehaviour", "You feel blessed.");
        datasl.SaveGame(spawnPointName);
        saveButtons.SetActive(false);
    }

    public void CancelSave()
    {
        notifText.text = "";
        saveButtons.SetActive(false);
    }
}
