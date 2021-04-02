using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class AspectOrb : MonoBehaviour
{
    private static class _colors {
        public static Color
            Ignis       = new Color(1, 0.35f, 0),
            Aqua        = new Color(0.24f, 0.83f, 0.98f),
            Terra       = new Color(0.35f, 0.75f, 0),
            Ordo        = new Color(0.85f, 0.83f, 0.92f),
            Aer         = new Color(1, 1, 0.5f),
            Praecantio  = new Color(0.695f, 0, 0.77f),
            Perditio    = new Color(0.25f, 0.25f, 0.25f),
            LightViolet = new Color(1, 0.83f, 0.98f)
        ;

        public static Color[] colors = {
            Ignis,
            Aqua,
            Terra,
            Ordo,
            Aer,
            Praecantio,
            Perditio,
            LightViolet
        };
	}

    public float floatAmp;
    public float floatSpeed;
    public GameObject floatingObj;
    private Vector2 f_pos;

    public float flickerAmp;
    public float flickerSpeed;

    public float lifetimeMin;
    public float lifetimeMax;
    private float _lifetime;
    private float _fadeoutTime;
    private float _cycleOffsetPos;
    private float _cycleOffsetFlic;

    private Light2D _lightSource;
    private float _currLL = 1;
    private Renderer _r;
    public float hdrScale;

    void Start()
    {
        f_pos = floatingObj.transform.localPosition;
        _lightSource = GetComponentInChildren<Light2D>();

        _lightSource.color = _colors.colors[Random.Range(0, _colors.colors.Length-1)];
        _cycleOffsetPos = Random.value;
        _cycleOffsetFlic = Random.value;

        _r = GetComponentInChildren<Renderer>();

        _r.material.color = _lightSource.color * new Color(hdrScale, hdrScale, hdrScale);

        _lifetime = Random.Range(lifetimeMin, lifetimeMax);
        _fadeoutTime = Random.Range(0.1f, 0.5f);

        StartCoroutine(fadeout());
    }

    private IEnumerator fadeout() {
        float dissolveTime = _fadeoutTime * _lifetime;
        yield return new WaitForSeconds(_lifetime * (1 - _fadeoutTime));

        var _r = GetComponentInChildren<Renderer>();

        float oldA = _r.material.color.a;
        var _c = _r.material.color;

        float _time = 0;

        while (_time < dissolveTime) {
            float dt = _time / dissolveTime;
            _c.a = Mathf.Lerp(oldA, 0, dt);
            _r.material.color = _c;
            _currLL = 1 - dt;

            _time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        _lightSource.intensity = _currLL * (1 - flickerAmp / 2 + Mathf.PingPong((Time.timeSinceLevelLoad + _cycleOffsetFlic) * flickerSpeed, flickerAmp));
        float _p = Mathf.PingPong((Time.timeSinceLevelLoad + _cycleOffsetPos) * floatSpeed, 1);
        f_pos.y = (_p * (1 - _p)) * floatAmp;
        floatingObj.transform.localPosition = f_pos;
    }
}
