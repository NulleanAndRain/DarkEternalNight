using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public string sceneName;
    public GameObject scorePanel;
    MenuControl mc;

    public Text CurrScore;
    public Text BestScore;
    public Text LastScore;


    private void Start() {
        mc = GetComponent<MenuControl>();
        if (!MenuData.willShowScore) {
            return;
        }

        mc.mainPanel.SetActive(false);
        mc.settingsPanel.SetActive(false);
        scorePanel.SetActive(true);

        setCurrScore(MenuData.Score);

        if (PlayerPrefs.HasKey("lastScore")) {
            setLastScore(PlayerPrefs.GetFloat("lastScore"));
        } else {
            setLastScore(0);
        }
        PlayerPrefs.SetFloat("lastScore", MenuData.Score);

        float best;
        if (PlayerPrefs.HasKey("bestScore")) {
            best = PlayerPrefs.GetFloat("bestScore");
            if (MenuData.Score > best) best = MenuData.Score;
        } else {
            best = MenuData.Score;
        }
        PlayerPrefs.SetFloat("bestScore", best);
        setBestScore(best);

        MenuData.willShowScore = false;
    }

    private void setCurrScore(float score) {
        CurrScore.text = $"Your score: {HUD.floatToString(score)}";
    }
    private void setBestScore(float score) {
        BestScore.text = $"Best score: {HUD.floatToString(score)}";
    }

    private void setLastScore(float score) {
        LastScore.text = $"Last score: {HUD.floatToString(score)}";
    }


    public void loadScene() {
        SceneManager.LoadScene(sceneName);
    }

    public void closeScorePanel() {
        mc.mainPanel.SetActive(true);
        scorePanel.SetActive(false);
    }
}

public static class MenuData {
    public static float Score;
    public static bool willShowScore = false;
}