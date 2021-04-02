using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public bool isPaused { get; private set; } = false;

    public static GameManager instance { get; private set; }

    public float _RespawnTime;
    public static float RespawnTime { get => instance._RespawnTime; }

    public Transform _RespawnPos;
    public static Transform RespawnPos { get => instance._RespawnPos; }

    //public GameObject LoadingScreen;
    void Awake() {
		//LoadingScreen.SetActive(true);
		if (instance != null) Destroy(gameObject);
        instance = this;

        Pause();
    }

    void Start() {
        StartCoroutine(waitLoad());
    }

    private IEnumerator waitLoad() {
        yield return new WaitForSecondsRealtime(0.5f);
        //LoadingScreen.SetActive(false);
        Resume();
    }

    public static void Resume() {
        instance.setPauseActive(false);
    }

    public static void Pause() {
        instance.setPauseActive(true);
    }

    void setPauseActive(bool state) {
		if (state) {
            isPaused = true;
            Time.timeScale = 0;
        } else {
            isPaused = false;
            Time.timeScale = 1;
        }
	}
}
