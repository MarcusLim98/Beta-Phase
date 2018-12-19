using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiBehaviour : MonoBehaviour {

    public GameObject fadeToBlack, fadeFromBlack;

    private void Start()
    {
        StartCoroutine(FadeFromBlack());
    }

    #region Quit Button
    public void ConfirmQuit(GameObject confirmBox)
    {
        confirmBox.SetActive(true);
    }

    public void CloseQuit(GameObject confirmBox)
    {
        confirmBox.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion

    private void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            StartCoroutine(FadeFromBlack());
        }
    }

    #region Scene Needs
    public void LoadScene(string nextScene)
    {
        StartCoroutine(FadeToScene(nextScene));
    }

    public void LoadGame()                                                  //loads the scene of the last checkpoint
    {
        if (PlayerPrefs.HasKey("spawnscene"))
        {
            LoadScene(PlayerPrefs.GetString("spawnscene"));
        }
        else
            LoadScene("Outside_Warehouse");                                 //TEMP FOR PRE-BETA
    }

    IEnumerator FadeToScene(string nextScene)
    {
        fadeToBlack.SetActive(true);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(nextScene);
    }

    IEnumerator FadeFromBlack()
    {
        yield return new WaitForSeconds(1);
        fadeFromBlack.SetActive(false);
    }
    #endregion
}
