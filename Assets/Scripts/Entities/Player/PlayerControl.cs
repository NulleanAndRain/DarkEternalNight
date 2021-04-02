using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WalkerComponent))]
public class PlayerControl : MonoBehaviour {
    WalkerComponent walker;
    void Start() {
        walker = GetComponent<WalkerComponent>();
    }

    Vector3 _lsc = Vector3.one;
    void Update() {
        float x = Input.GetAxis("Horizontal");

        if (x != 0) {
            walker.moveDir(x);
            if (x < 0 && transform.localScale.x > 0 || x >0 && transform.localScale.x < 0) {
                _lsc = transform.localScale;
                _lsc.x *= -1;
                transform.localScale = _lsc;
			}
		}

        if (Input.GetKeyDown(KeyCode.Space))
            walker.jump();
    }
}
