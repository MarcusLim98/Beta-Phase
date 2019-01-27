using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FaderLogic : MonoBehaviour {

    public GameObject fadeToClear;
    public GameObject fadeToBlack;
    DataSaveAndLoad datasl;
    public string nextLevel;
    public bool restartFromTheStart, affectsPlayer;
    string spawnPointName;
    // Use this for initialization
    void Start () {
        datasl = GameObject.Find("GameController").GetComponent<DataSaveAndLoad>();
        StartCoroutine(DisableFadeIn());
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.O))
        {
            fadeToClear.SetActive(true);
            StartCoroutine(DisableFadeIn());
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            fadeToBlack.SetActive(true);
            StartCoroutine(DisableFadeOut());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !restartFromTheStart && !affectsPlayer)
        {
            fadeToBlack.SetActive(true);
            //datasl.SaveGame(spawnPointName);
            StartCoroutine(DisableFadeOut());
        }

        if (other.tag == "Player" && restartFromTheStart && !affectsPlayer)
        {
            print("restart");
            fadeToBlack.SetActive(true);
            StartCoroutine(Restart());
        }
        if (other.name == "LaoDaBullet(Clone)" && affectsPlayer)
        {
            print("dead");
            fadeToBlack.SetActive(true);
            StartCoroutine(DisableFadeOut());
        }
    }

    IEnumerator DisableFadeIn()
    {
        yield return new WaitForSeconds(1f);
        fadeToClear.SetActive(false);
    }

    IEnumerator DisableFadeOut()
    {
        yield return new WaitForSeconds(2f);
        if (this.gameObject.name == "LosingCondiiton" || this.gameObject.name == "Player")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (this.gameObject.name == "EndGame")
        {
            SceneManager.LoadScene(nextLevel);
        }
        if (this.gameObject.name == "EndGame1")
        {
            SceneManager.LoadScene("Scene 7 ABHouse");
        }
        //fadeToBlack.SetActive(false);
    }

    IEnumerator Restart()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
