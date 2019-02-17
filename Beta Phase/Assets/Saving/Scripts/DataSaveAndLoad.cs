using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataSaveAndLoad : MonoBehaviour {

    public GameObject playerObj;
    Vector3 spawnPos;
    static DataSaveAndLoad data;
    public static List<KeyItem> keyItemList = new List<KeyItem>();
    public NavMeshAgent playerAgent;
    Text objectiveText;

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

        CreateItemList();
        CheckItem();
        //foreach (KeyItem item in keyItemList)                                       //check through entire item list
        //{
        //    print(item.keyItemName + ": " + item.taken);
        //}
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            LoadData();
        }
    }

    private void OnLevelWasLoaded(int level)                                        //spawns player at last checkpoint in playable levels
    {
        if (level > 0)
        {
            LoadData();
        }
    }

    void LoadData()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        playerAgent = playerObj.GetComponent<NavMeshAgent>();
        objectiveText = GameObject.Find("ObjectiveText").GetComponent<Text>();

        //if player has saved in this specific scene
        if (PlayerPrefs.HasKey("spawnpoint") && SceneManager.GetActiveScene().name == PlayerPrefs.GetString("spawnscene"))
        {
            spawnPos = GameObject.Find(PlayerPrefs.GetString("spawnpoint")).transform.position;
            playerObj.transform.position = new Vector3(spawnPos.x, spawnPos.y, spawnPos.z);
            playerAgent.enabled = true;
            objectiveText.text = PlayerPrefs.GetString("savedobjective");
        }
        else return;
    }

    public void SaveGame(string spawnPointName)
    {
        PlayerPrefs.SetString("spawnpoint", spawnPointName);                        //saves name of last checkpoint
        PlayerPrefs.SetString("spawnscene", SceneManager.GetActiveScene().name);    //saves name of last scene
        PlayerPrefs.SetString("savedobjective", objectiveText.text);                //saves last objective

        foreach (KeyItem item in keyItemList)                                       //check through entire item list
        {
            PlayerPrefs.SetInt(item.keyItemName, item.taken);                       //if taken, save the value as 1 (true)
            print(item.keyItemName + ": " + item.taken);
        }

        PlayerPrefs.Save();
    }

    void CreateItemList()
    {
        //Tutorials
        //keyItemList.Add(new KeyItem("TutorialEave", 0));
        keyItemList.Add(new KeyItem("TutorialSave", 0));
        keyItemList.Add(new KeyItem("TutorialStealth", 0));
        keyItemList.Add(new KeyItem("TutorialNoise", 0));
        keyItemList.Add(new KeyItem("TutorialSpotter", 0));

        //Scene 1
        keyItemList.Add(new KeyItem("Day1Afterwork", 0));
        keyItemList.Add(new KeyItem("Day1AfterEavesdrop", 0));
        keyItemList.Add(new KeyItem("Day1AfterCShopEavesdrop", 0));

        //Scene 2
        keyItemList.Add(new KeyItem("Day1AfterGambleEavesdrop", 0));

        //Scene 3
        keyItemList.Add(new KeyItem("Day2AfterWork", 0));
        keyItemList.Add(new KeyItem("Day2PanToThugs", 0));

        //Scene 4
        keyItemList.Add(new KeyItem("AfterEaveCall", 0));
        keyItemList.Add(new KeyItem("AfterEaveDocs", 0));
        //keyItemList.Add(new KeyItem("Day2Spotter", 0));
        keyItemList.Add(new KeyItem("WHDocs", 0));

        //Scene 8
        keyItemList.Add(new KeyItem("BossPhase", 0));                               // 0 - nothing // 1 - forced eave1 // 2 - picked up first doc // 3 - intro // 4 - eave 2 // 5 - eave 3
        keyItemList.Add(new KeyItem("BossIntro", 0));
        keyItemList.Add(new KeyItem("BossOutro", 0));
        keyItemList.Add(new KeyItem("BossEaveNo1", 0));
        keyItemList.Add(new KeyItem("BossEaveNo2", 0));
        keyItemList.Add(new KeyItem("BossEaveNo3", 0));
        keyItemList.Add(new KeyItem("BossDocs", 0));
        keyItemList.Add(new KeyItem("Documents - 1", 0));
        keyItemList.Add(new KeyItem("Documents - 2", 0));
        keyItemList.Add(new KeyItem("Documents - 3", 0));
        keyItemList.Add(new KeyItem("Documents - 4", 0));
        keyItemList.Add(new KeyItem("Documents - 5", 0));
        keyItemList.Add(new KeyItem("Documents - 6", 0));
    }

    public void ObtainItem(string foundItemName, int number)                        //for obtaining items
    {
        foreach (KeyItem item in keyItemList)                                       //check through entire item list
        {
            if (item.keyItemName == foundItemName)                                  //if an item matches the interacted object name
            {
                item.taken = number;                                                //say that item has been taken
            }
        }
    }

    void CheckItem()                                                                //checks item status based on last save
    {
        foreach (KeyItem item in keyItemList)                                       //check through entire item list
        {
            if (PlayerPrefs.HasKey(item.keyItemName))
            {
                item.taken = PlayerPrefs.GetInt(item.keyItemName);                  //assigns saved value to current taken value
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))                                       //to clear all data keys
        {
            PlayerPrefs.DeleteAll();
            foreach (KeyItem item in keyItemList)                                   //check through entire item list
            {
                PlayerPrefs.SetInt(item.keyItemName, 0);                            //if taken, save the value as 0 (false)
            }
            print("All keys cleared");
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))                                       //testing purposes only
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

}
