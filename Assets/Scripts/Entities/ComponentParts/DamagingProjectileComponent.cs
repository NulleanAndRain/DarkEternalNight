using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingProjectileComponent : MonoBehaviour {

	public float critChance;
	public float critMultipler;
	public float knockbackInflictedMultipler;

	private List<GameObject> damagedObjects = new List<GameObject>();

	public void damageTarget(GameObject target, float damage, Vector3 center) {
		if (damagedObjects.Contains(target)) return;
		damagedObjects.Add(target);

		Health health = target.GetComponent<Health>();
		if (health == null) return;

		bool isCrit = false;
		if (Random.value < critChance) {
			damage *= (1 + critMultipler);
			isCrit = true;
		}

		health.GetDamage(damage, transform.position + center, isCrit, knockbackInflictedMultipler);
	}
	public void damageTarget(GameObject target, float damage) {
		damageTarget(target, damage, Vector2.zero);
	}

	public void clearDamagedList() {
		damagedObjects.Clear();
	}


	public bool listContains(GameObject obj) => damagedObjects.Contains(obj);
}
