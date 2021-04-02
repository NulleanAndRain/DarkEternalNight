using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollectableItem))]
public class HealCollectable : MonoBehaviour {
    [Min(0)]
    public float HealAmount;

    void Start() {
        CollectableItem collectable = GetComponent<CollectableItem>();
        void collect(Collider2D coll) {
            var h = coll.GetComponent<Health>();
            if (h == null) return;
            h.Heal(HealAmount);
            collectable.collect();
        }
        collectable.onCollect += collect;
    }
}
