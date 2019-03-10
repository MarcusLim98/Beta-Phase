using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiBehaviour : MonoBehaviour {

    public GameObject fadeToBlack, fadeFromBlack, loadingScreen;
    public Slider loadingBar, voiceSlider, soundSlider;
    DataSaveAndLoad dsal;
    public static float voiceVolume, soundVolume;
    public AudioSource[] sounds;

    private void Start()
    {

        if (SceneManager.GetActiveScene().name == "FakeMenu")
        {
            Button resumeBtn = GameObject.Find("Resume").GetComponent<Button>();
            if (!PlayerPrefs.HasKey("spawnscene"))
            {
                resumeBtn.interactable = false;
            } else { resumeBtn.interactable = true; }
            voiceSlider.value = PlayerPrefs.GetFloat("Voice", voiceSlider.value);
            voiceSlider.value = voiceVolume;
            soundSlider.value = PlayerPrefs.GetFloat("Sound", soundSlider.value);
            soundSlider.value = soundVolume;
        }

        dsal = GameObject.Find("DataController").GetComponent<DataSaveAndLoad>();
        //print(voiceVolume);

        sounds = FindObjectsOfType<AudioSource>();
        foreach (AudioSource sfx in sounds)
        {
            if (sfx.name != "RPGTalk")
            {
                sfx.volume = soundVolume;
            }
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

    public void LoadGame()                                                  //loads the scene of the last checkpoint
    {
        if (PlayerPrefs.HasKey("spawnscene"))
        {
            LoadScene(PlayerPrefs.GetString("spawnscene"));
        }
    }

    public void NewGame()
    {
        dsal.ClearData();                                                   //clears past progress
        LoadScene("Scene 0 Police Office");
    }

    IEnumerator FadeToScene(string nextScene)
    {
        loadingScreen.SetActive(true);

        yield return new WaitForSeconds(1);                                 //so that fadeToBlack settles

        AsyncOperation ao = SceneManager.LoadSceneAsync(nextScene);
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            loadingBar.value = ao.progress;                                 //change bar value

            if (ao.progress >= 0.9f)                                        //if loading is done
            {
                fadeToBlack.SetActive(true);
                yield return new WaitForSeconds(1);                         //so that fadeToBlack settles
                ao.allowSceneActivation = true;                             //start next scene
            }

            yield return null;
        }
    }

    IEnumerator FadeFromBlack()                                             //ensures that canvas does not block input
    {
        yield return new WaitForSeconds(2);
        fadeFromBlack.SetActive(false);
    }
    #endregion

    public void SaveVoice()
    {
        PlayerPrefs.SetFloat("Voice", voiceSlider.value);
        voiceVolume = voiceSlider.value;
        PlayerPrefs.Save();
    }

    public void SaveSound()
    {
        PlayerPrefs.SetFloat("Sound", soundSlider.value);
        soundVolume = soundSlider.value;
        foreach (AudioSource sfx in sounds)
        {
            if (sfx.name != "RPGTalk")
            {
                sfx.volume = soundVolume;
            }
        }
        PlayerPrefs.Save();
    }

}
