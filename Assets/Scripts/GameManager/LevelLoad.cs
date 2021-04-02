using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelLoad : MonoBehaviour
{
    public string sceneName;

    public static LevelLoad instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void loadScene() {
        SceneManager.LoadScene(sceneName);
    }

    public void quitGame() {
        Application.Quit();
	}
}
