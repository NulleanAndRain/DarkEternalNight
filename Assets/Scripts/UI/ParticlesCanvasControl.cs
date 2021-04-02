using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesCanvasControl : MonoBehaviour {
    public static ParticlesCanvasControl instance;

    public GameObject _deathSplash;

    void Start() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public static void createDeathSplash() {
        var _instance = Instantiate(instance._deathSplash, instance._deathSplash.transform.position, Quaternion.identity);

        _instance.transform.SetParent(instance.transform, true);
        _instance.transform.localScale = Vector3.one;

        var dissolve = _instance.GetComponent<DeathSplash>();
        dissolve.startDissolve();
    }
}
