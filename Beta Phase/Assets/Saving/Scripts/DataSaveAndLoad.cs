using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class DataSaveAndLoad : MonoBehaviour {

    public GameObject playerObj;
    Vector3 spawnPos;
    static DataSaveAndLoad data;
    //public static List<KeyItem> keyItemList = new List<KeyItem>();
    public NavMeshAgent playerAgent;

    void Awake()                                                                    //ensures that this script is present in every scene
    {
        if (data != null && data != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            data = this;
        }
        DontDestroyOnLoad(this.gameObject);

        //CreateItemList();
        //CheckItem();
    }

    private void OnLevelWasLoaded(int level)                                        //spawns player at last checkpoint in playable levels
    {
        if (level > 0)
        {
            playerObj = GameObject.FindGameObjectWithTag("Player");
            playerAgent = playerObj.GetComponent<NavMeshAgent>();

            if (PlayerPrefs.HasKey("spawnpoint") && SceneManager.GetActiveScene().name == PlayerPrefs.GetString("spawnscene"))
            {
                spawnPos = GameObject.Find(PlayerPrefs.GetString("spawnpoint")).transform.position;
                playerObj.transform.position = new Vector3(spawnPos.x, spawnPos.y, spawnPos.z);
                playerAgent.enabled = true;
            }
            else return;
        }
    }

    public void SaveGame(string spawnPointName)
    {
        PlayerPrefs.SetString("spawnpoint", spawnPointName);                        //saves name of last checkpoint
        PlayerPrefs.SetString("spawnscene", SceneManager.GetActiveScene().name);    //saves name of last scene

        //foreach (KeyItem item in keyItemList)                                       //check through entire item list
        //{
        //    PlayerPrefs.SetInt(item.keyItemName, item.taken);                       //if taken, save the value as 1 (true)
        //}

        PlayerPrefs.Save();
    }

    //void CreateItemList()
    //{
    //    keyItemList.Add(new KeyItem("Day1Intro", 0));
    //    keyItemList.Add(new KeyItem("Day1DonePapers", 0));
    //    keyItemList.Add(new KeyItem("Day1Afterwork", 0));
    //    keyItemList.Add(new KeyItem("Day1Suspicion", 0));
    //    keyItemList.Add(new KeyItem("Day1ApproachThugs", 0));
    //    keyItemList.Add(new KeyItem("Day2Intro", 0));
    //    keyItemList.Add(new KeyItem("Day2AfterWork", 0));
    //    keyItemList.Add(new KeyItem("Day2Notice", 0));
    //    keyItemList.Add(new KeyItem("Day3Intro", 0));
    //    keyItemList.Add(new KeyItem("Day3AfterWork", 0));
    //    keyItemList.Add(new KeyItem("LaoDaIntro", 0));
    //    keyItemList.Add(new KeyItem("LaoDaDefeat", 0));
    //}

    //public void ObtainItem(string foundItemName)                                    //for obtaining items
    //{
    //    foreach (KeyItem item in keyItemList)                                       //check through entire item list
    //    {
    //        if (item.keyItemName == foundItemName)                                  //if an item matches the interacted object name
    //        {
    //            item.taken = 1;                                                     //say that item has been taken
    //        }
    //    }
    //}

    //void CheckItem()                                                                //checks item status based on last save
    //{
    //    foreach (KeyItem item in keyItemList)                                       //check through entire item list
    //    {
    //        if (PlayerPrefs.HasKey(item.keyItemName))
    //        {
    //            item.taken = PlayerPrefs.GetInt(item.keyItemName);                  //assigns saved value to current taken value
    //        }
    //    }
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))                                       //to clear all data keys
        {
            PlayerPrefs.DeleteAll();
            print("All keys cleared");
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))                                       //testing purposes only
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

}
