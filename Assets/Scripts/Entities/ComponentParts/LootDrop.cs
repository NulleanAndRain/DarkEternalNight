using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct DropableItem {
    public GameObject collectableItemPrefab;
    public float dropChance;
    public float minDropAmount;
    public float maxDropAmount;
    public bool useDropAmount;
    public bool useBothDrops;
}

[RequireComponent(typeof(Health))]
public class LootDrop : MonoBehaviour {
    public Vector2 dropPoint;
    public float vertSpeed;
    public float horSpeedRange;
    [SerializeField]
    public DropableItem[] lootList;

    private bool lootTriggered = false;

    private List<GameObject> _items = new List<GameObject>();

    public bool usedByOtherScript;

    void Start() {
        if (!usedByOtherScript) {
            var h = GetComponent<Health>();
            h.onDowned += dropLoot;
        }
    }

    public void resetTrigger() {
        lootTriggered = false;
        _items.Clear();
    }

    public void dropLoot() {
        if (!enabled || lootTriggered) return;
        lootTriggered = true;
        foreach (var item in lootList) {
            if (item.useDropAmount || item.useBothDrops) {
                float amount = Random.Range(item.minDropAmount, item.maxDropAmount);

                for (int i = 0; i < Mathf.FloorToInt(amount); i++) {
                    _items.Add(item.collectableItemPrefab);
                }
                if (Random.value < amount % 1) _items.Add(item.collectableItemPrefab);
            }
            if (item.useBothDrops || !item.useDropAmount) {

                for (int i = 0; i < Mathf.FloorToInt(item.dropChance); i++) {
                    _items.Add(item.collectableItemPrefab);
                }
                if (Random.value < item.dropChance % 1) _items.Add(item.collectableItemPrefab);
            }
        }

        foreach (var item in _items) {
            var instance = Instantiate(item, (Vector2)transform.position + dropPoint, item.transform.rotation);
            var rb = instance.GetComponent<Rigidbody2D>();

            if (rb != null) {
                rb.velocity = new Vector2(Random.Range(-horSpeedRange, horSpeedRange), vertSpeed);
            }
        }
	}
}
