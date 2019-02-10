using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiBehaviour : MonoBehaviour {

    public GameObject fadeToBlack, fadeFromBlack;

    private void Start()
    {

        if (SceneManager.GetActiveScene().name == "FakeMenu")
        {
            Button resumeBtn = GameObject.Find("Resume").GetComponent<Button>();
            if (!PlayerPrefs.HasKey("spawnscene"))
            {
                resumeBtn.interactable = false;
            } else { resumeBtn.interactable = true; }
        }

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

    #region Scene Needs
    public void LoadScene(string nextScene)
    {
        StartCoroutine(FadeToScene(nextScene));
    }

    public void LoadGame()                                              //loads the scene of the last checkpoint
    {
        if (PlayerPrefs.HasKey("spawnscene"))
        {
            LoadScene(PlayerPrefs.GetString("spawnscene"));
        }
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        LoadScene("Scene 0 Police Office");
    }

    IEnumerator FadeToScene(string nextScene)
    {
        fadeToBlack.SetActive(true);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(nextScene);
    }

    IEnumerator FadeFromBlack()                                             //ensures that canvas does not block input
    {
        yield return new WaitForSeconds(2);
        fadeFromBlack.SetActive(false);
    }
    #endregion
}
