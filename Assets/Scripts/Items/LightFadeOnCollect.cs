using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightFadeOnCollect : MonoBehaviour {

    void Start() {
        CollectableItem collectable = GetComponent<CollectableItem>();
        void onDissolve() {
            StartCoroutine(fadeout(collectable.DissolveDuration));
		}
        collectable.onDissolveStart += onDissolve;
    }

    private IEnumerator fadeout(float time) {
        float _time = 0;
        Light2D ls = GetComponentInChildren<Light2D>();

        float _oldInt = ls.intensity;

        while (_time < time) {
            float _t = _time / time;

            ls.intensity = Mathf.Lerp(_oldInt, 0, _t);

            _time += Time.fixedDeltaTime;
            yield return new WaitForEndOfFrame();
		}
	}
}
