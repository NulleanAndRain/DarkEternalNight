using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private static HUD _hud;

    public Text Score;
    public Text HealthNum;
    //public Image HealthBar;
    public Image HealthLine;
    public Image SkillCD;

    public GameObject _defaultTextParticle;
    public static GameObject defaultTextParticle { get => _hud._defaultTextParticle; }

    // Start is called before the first frame update
    void Start()
    {
        if (_hud != null) Destroy(gameObject);
        _hud = this;
    }

    public static void updateScore(string score) {
        int addAmn = 6 - score.Length;
        for (int i = 0; i < addAmn; i++) score = "0" + score;
        _hud.Score.text = score;
	}

    private static Vector3 _sc = Vector3.one;
    public static void updateHealth(float curr, float max) {
        float perc = curr / max;
        _sc.x = perc;
        _hud.HealthLine.rectTransform.localScale = _sc;
        _hud.HealthNum.text = floatToString(curr) + '/' + floatToString(max);
    }

    private static Vector3 _sc1 = Vector3.one;
    public static void updateSkillCD(float percentage) {
        _sc1.y = percentage;
        _hud.SkillCD.transform.localScale = _sc1;
    }



    //
    public static string floatToString(float score) => Math.Round(score, 1, MidpointRounding.AwayFromZero).ToString();
}
