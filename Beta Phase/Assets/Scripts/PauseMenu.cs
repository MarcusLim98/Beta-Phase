using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public GameObject blurBG;
    public Canvas inGameCanvas;
    public Canvas pauseMenu;
    [HideInInspector]
    public bool isPaused;
    UiBehaviour uiB;

	void Start () {
        uiB = GameObject.Find("UiBehaviour").GetComponent<UiBehaviour>();
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            isPaused = true;
            blurBG.SetActive(true);
            inGameCanvas.enabled = false;
            pauseMenu.enabled = true;
            Time.timeScale = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            isPaused = false;
            blurBG.SetActive(false);
            inGameCanvas.enabled = true;
            pauseMenu.enabled = false;
            Time.timeScale = 1;
        }
    }

    public void Resume()
    {
        isPaused = false;
        blurBG.SetActive(false);
        inGameCanvas.enabled = true;
        pauseMenu.enabled = false;
        Time.timeScale = 1;
    }

    public void LoadLevel(string name)
    {
        Time.timeScale = 1;
        //SceneManager.LoadScene(name);
        uiB.LoadScene(name);
    }
}
