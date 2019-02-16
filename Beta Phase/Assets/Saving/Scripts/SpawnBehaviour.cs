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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "EndGame")
        {
            ui.LoadScene(nextLevel);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("KeyItem") && Input.GetKeyDown(KeyCode.E))
        {
            StopCoroutine("NotifTextBehaviour");
            StartCoroutine("NotifTextBehaviour", other.name + " obtained");
        }

        //if (other.name == "EndGame")
        //{
        //    ui.LoadScene(nextLevel);
        //}

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

}
