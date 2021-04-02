using UnityEngine;

public class PauseControl : MonoBehaviour {
    public GameObject pauseUI;
    public GameObject HUD;
    public MenuControl menuControl;
    private static PauseControl _instance;
    public static bool isPaused { get => _instance.pauseUI.activeSelf; }

    private void Start() {
        if (_instance != null) {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        if (isPaused) {
            resume();
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isPaused) {
                resume();
            } else {
                pause();
            }
        }
    }

    public void pause() {
        pauseUI.SetActive(true);
        HUD.SetActive(false);
        Time.timeScale = 0f;
        menuControl.openMainPanel();
    }

    public void resume() {
        pauseUI.SetActive(false);
        HUD.SetActive(true);
        Time.timeScale = 1f;
    }
}
