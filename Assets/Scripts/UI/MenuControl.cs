using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuControl : MonoBehaviour {
    const string mainMenuScene = "MainMenu";

    public GameObject mainPanel;
    public GameObject settingsPanel;

    [Header("Settings")]
    public AudioMixerGroup mixer;
    public Scrollbar masterVolumeScrollbar;

    private float soundPow = 0.3f;

    private void Start() {
        if (PlayerPrefs.HasKey("MasterVolume")) {
            float masterVol = PlayerPrefs.GetFloat("MasterVolume");
            setMasterVolume(masterVol);
            masterVolumeScrollbar.value = masterVol;
        } else {
            PlayerPrefs.SetFloat("MasterVolume", 1);
        }
    }
    public void openSettings() {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void closeSettings() {
        openMainPanel();
    }

    void setMasterVolume(float volume) {
        mixer.audioMixer.SetFloat("Master", Mathf.Lerp(-80, 0, Mathf.Pow(volume, soundPow)));
    }

    public void changeMasterVolume() {
        float val = masterVolumeScrollbar.value;
        setMasterVolume(val);
        PlayerPrefs.SetFloat("MasterVolume", val);
    }

    public void exitToMainMenu() {
        exitToMainMenu(GameObject.FindGameObjectWithTag("Player"));
    }

    public void exitToMainMenu(GameObject player) {
        MenuData.Score = player.GetComponent<ScoreCollector>().Score;
        MenuData.willShowScore = true;
        SceneManager.LoadScene(mainMenuScene);
    }


    public void quitGame() {
        Application.Quit();
    }

    public void openMainPanel() {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }
}
