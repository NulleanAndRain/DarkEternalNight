using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DeathSplash : MonoBehaviour {
    public float dissolveTime;

    public event Action onDissolveStart = delegate { };

    Text _t;
    public void startDissolve() {
        _t = GetComponent<Text>();

        _t.rectTransform.offsetMin = Vector2.zero;
        _t.rectTransform.offsetMax = Vector2.zero;
		StartCoroutine(startDissolveCoroutine());
	}

    public IEnumerator startDissolveCoroutine() {
        var mat_ref = _t.material;
        _t.material = Instantiate(mat_ref);

        yield return new WaitForSeconds(GameManager.RespawnTime - dissolveTime);
        onDissolveStart();

        float oldA = _t.material.color.a;
        var _c = _t.material.color;
        float _time = 0;

        while (_time < dissolveTime) {
            float dt = _time / dissolveTime;
            _c.a = Mathf.Lerp(oldA, 0, dt);
            _t.material.color = _c;

            _time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
