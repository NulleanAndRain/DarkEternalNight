using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveBeforeDestroyComponent : MonoBehaviour {
    public float timeBeforeDissolve;
    public float dissolveTime;
    public GameObject deathParticlesPrefab;
    public Vector2 deathParticlesPoint;

    public event Action onDissolveStart = delegate { };

    public void startDissolve() {
        StartCoroutine(startDissolveCoroutine());
	}

    public IEnumerator startDissolveCoroutine() {
        GameObject _pInst = null;
        if (deathParticlesPrefab != null)
            _pInst = Instantiate(
                deathParticlesPrefab,
                (Vector2)transform.position + deathParticlesPoint,
                deathParticlesPrefab.transform.rotation);

        yield return new WaitForSeconds(timeBeforeDissolve);
        onDissolveStart();

        var _r = GetComponent<Renderer>();
        float oldA = _r.material.color.a;
        var _c = _r.material.color;
        float _time = 0;

        while (_time < dissolveTime) {
            float dt = _time / dissolveTime;
            _c.a = Mathf.Lerp(oldA, 0, dt);
            _r.material.color = _c;

            _time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
        if (_pInst != null)
            Destroy(_pInst);
    }
}
