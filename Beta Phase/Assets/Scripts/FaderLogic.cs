using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FaderLogic : MonoBehaviour {

    public GameObject fadeToClear;
    public GameObject fadeToBlack;
    DataSaveAndLoad datasl;
    public string nextLevel;
    public bool restartFromTheStart;
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
        if(other.tag == "Player" && !restartFromTheStart)
        {
            fadeToBlack.SetActive(true);
            //datasl.SaveGame(spawnPointName);
            StartCoroutine(DisableFadeOut());
        }

        if (other.tag == "Player" && restartFromTheStart)
        {
            print("restart");
            fadeToBlack.SetActive(true);
            StartCoroutine(Restart());
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
        if (this.gameObject.name == "LosingCondiiton")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (this.gameObject.name == "EndGame")
        {
            SceneManager.LoadScene(nextLevel);
        }
        //fadeToBlack.SetActive(false);
    }

    IEnumerator Restart()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
