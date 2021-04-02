using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextParticle : MonoBehaviour
{
    public string msg;
    [HideInInspector]
    public Vector2 worldPos;
    public Vector2 movement;

    public Color mainColor;
    public Color dissolveColor1;
    public Color dissolveColor2;

    public float moveTime;
    public float idleTime;
    public float dissolveTime1;
    public float dissolveTime2;

    private Coroutine _coroutMove;
    private Coroutine _coroutDiss;
    private Material mat_ref;

    private Text _text;

    void Start() {
        _text = GetComponent<Text>();
        worldPos = transform.position;

        mat_ref = _text.material;
        _text.material = Instantiate(mat_ref);

        _text.text = msg;
        _text.color = mainColor;

        _coroutMove = StartCoroutine(move());
        _coroutDiss = StartCoroutine(dissolve());
    }

    private IEnumerator move() {
        Vector2 startPos = worldPos;
        float _time = 0;
        while (_time < moveTime) {
            float dt = _time / moveTime;

            worldPos = Vector2.Lerp(startPos, startPos + movement, dt);

            _time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator dissolve() {

        yield return new WaitForEndOfFrame();

        _text.text = msg;
        _text.color = mainColor;

        yield return new WaitForSeconds(moveTime + idleTime);

        float _time = 0;
        while (_time < dissolveTime1) {
            float dt = _time / dissolveTime1;

            _text.material.color = Color.Lerp(mainColor, dissolveColor1, dt);

            _time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _time = 0;
        while (_time < dissolveTime2) {
            float dt = _time / dissolveTime2;

            _text.material.color = Color.Lerp(dissolveColor1, dissolveColor2, dt);

            _time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
		Destroy(gameObject);
	}

    public void updateTextColor(string newText, Color color) {
        mainColor = color;
        updateText(newText);

    }

    public void updateText(string newText) {
        msg = newText;
        resetTime();
	}

    public void resetTime() {
        if (_coroutDiss != null) StopCoroutine(_coroutDiss);
        _coroutDiss = StartCoroutine(dissolve());
    }

	void Update() {
		transform.position = worldPos;
	}

    public static TextParticle Instantiate(GameObject prefab, Vector2 pos, string msg) {
        var instance = Instantiate(prefab, pos, Quaternion.identity);
        instance.transform.SetParent(ParticlesCanvasControl.instance.transform, true);
        instance.transform.localScale = Vector3.one;

        TextParticle particle = instance.GetComponent<TextParticle>();
        particle.msg = msg;

        return particle;
	}
}
