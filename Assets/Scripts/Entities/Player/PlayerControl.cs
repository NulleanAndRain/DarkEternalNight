using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WalkerComponent), typeof(Animator))]
public class PlayerControl : MonoBehaviour {
    WalkerComponent walker;
    Animator anim;

    bool isFacingLeft;
    void Start() {
        walker = GetComponent<WalkerComponent>();
        anim = GetComponent<Animator>();
    }

    Vector3 _lsc;
    void Update() {
        float x = Input.GetAxis("Horizontal");

        if (Mathf.Abs(x) >= 1e-4) {
            walker.moveDir(x);
            if (x < 0 && !isFacingLeft || x >0 && isFacingLeft) {
                isFacingLeft = !isFacingLeft;
                _lsc = transform.localScale;
                _lsc.x *= -1;
                transform.localScale = _lsc;
            }

            anim.SetBool("isWalking", true);
		} else {
            anim.SetBool("isWalking", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
            walker.jump();
    }
}
