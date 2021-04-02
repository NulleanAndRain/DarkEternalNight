using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMagnet : MonoBehaviour {
    public Vector2 MagnetPos;
    public Vector2 MagnetPoint { get => (Vector2)transform.position + MagnetPos; }
	public float MagnetRadius;


	private void OnDrawGizmos() {
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(MagnetPoint, MagnetRadius);
	}

	private void FixedUpdate() {
		foreach (var c in Physics2D.OverlapCircleAll(MagnetPoint, MagnetRadius)) {
			var item = c.GetComponent<MagnetableItem>();
			if (item == null) continue;
			StartCoroutine(waitEdnOfFrame(item));
		}
	}

	IEnumerator waitEdnOfFrame(MagnetableItem item) {
		yield return new WaitForEndOfFrame();
		item.dragTo(this);
	}
}
