using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public GameObject blurBG;
    public Canvas inGameCanvas;
    public Canvas pauseMenu;
    bool isPaused;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
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
        SceneManager.LoadScene(name);
    }
}
