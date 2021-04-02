using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSettings : MonoBehaviour {
    public float gravity;

    void Start() {
        Physics2D.gravity = new Vector2(0, -gravity);
    }
}
