using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpawnBehaviour : MonoBehaviour {

    DataSaveAndLoad datasl;
    public Text notifText;
    [SerializeField]
    public string spawnPointName;
    public UiBehaviour ui;
    public AudioSource altarSound;
    public string nextLevel;
    string altarS;

    void Awake()
    {
        datasl = GameObject.Find("DataController").GetComponent<DataSaveAndLoad>();
        //PlayerPrefs.DeleteAll();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("SpawnPoint"))
    //    {
    //        spawnPointName = other.transform.GetChild(0).name;
    //        print(spawnPointName);
    //    }
    //}

    void OnTriggerStay(Collider other)
    {
        

        //if (other.CompareTag("SpawnPoint") && Input.GetKeyDown(KeyCode.E))
        //{
        //    SaveChoiceText();
        //    print("save now");
        //    spawnPointName = other.transform.GetChild(0).name;
        //    print(spawnPointName);
        //    SaveChoiceText();
        //}

        if (other.CompareTag("KeyItem") && Input.GetKeyDown(KeyCode.E))
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
                        //else if (item.taken == 1) { print("help"); StopCoroutine("NotifTextBehaviour"); StartCoroutine("NotifTextBehaviour", "Nothing here, huh."); }
                    }
                }
            }
        }

        if (other.name == "Thug")                               //use name, if not listening colliders will affect player
        {
            ui.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (other.name == "EndGame")
        {
            ui.LoadScene(nextLevel);
        }

        else return;
    }

    public IEnumerator NotifTextBehaviour(string notif)
    {
        notifText.text = notif;
        yield return new WaitForSeconds(3);
        notifText.text = "";
    }



    public void AutoSave()
    {
        StopCoroutine("NotifTextBehaviour");
        StartCoroutine("NotifTextBehaviour", "Autosaving...");
        datasl.SaveGame(spawnPointName);
        altarS = "Altar";
        altarSound.clip = (AudioClip)Resources.Load(altarS);
        altarSound.Play();
    }



    //void SaveChoiceText()
    //{
    //    StopCoroutine("NotifTextBehaviour");
    //    notifText.text = "It's an altar for worship. Save progress?";
    //    saveButtons.SetActive(true);
    //    print("it is saving");
    //    datasl.SaveGame(spawnPointName);
    //}

    //public void ConfirmSave()
    //{
    //    print("Saved!");
    //    StopCoroutine("NotifTextBehaviour");
    //    StartCoroutine("NotifTextBehaviour", "You feel blessed.");

    //    altarS = "Altar";
    //    altarSound.clip = (AudioClip)Resources.Load(altarS);
    //    altarSound.Play();
    //    datasl.SaveGame(spawnPointName);
    //    saveButtons.SetActive(false);
    //}

    //public void CancelSave()
    //{
    //    notifText.text = "";
    //    saveButtons.SetActive(false);
    //}
}
