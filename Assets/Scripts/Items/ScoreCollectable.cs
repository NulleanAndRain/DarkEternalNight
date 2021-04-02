using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollectableItem))]
public class ScoreCollectable : MonoBehaviour {

    public float Score;

    void Start() {
        CollectableItem collectable = GetComponent<CollectableItem>();
        void collect(Collider2D other) {
            var collector = other.GetComponent<ScoreCollector>();
            if (collector == null) return;
            collector.CollectScore(Score);
            collectable.collect();
        }
        collectable.onCollect += collect;
    }
}
