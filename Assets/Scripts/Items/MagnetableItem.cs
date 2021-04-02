using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CollectableItem))]
public class MagnetableItem : MonoBehaviour {
	private Rigidbody2D rb;
	private CollectableItem item;
	public Collider2D _GroundCollider;

	private List<ItemMagnet> magnets = new List<ItemMagnet>();
	private bool isDragging;

	private void Start() {
		rb = GetComponent<Rigidbody2D>();
		item = GetComponent<CollectableItem>();

		void disable(Collider2D _) {
			enabled = false;
		}

		item.onCollect += disable;
	}

	public void dragTo(ItemMagnet magnet) {
		magnets.Add(magnet);
	}

	private Vector2 _vel;
	private void FixedUpdate() {
		if (magnets.Count == 0) {
			if (isDragging) {
				rb.gravityScale = 1;
				rb.velocity = Vector2.zero;
				_GroundCollider.enabled = true;
				isDragging = false;
			}
			return;
		}

		isDragging = true;
		_GroundCollider.enabled = false;

		ItemMagnet closest = null;
		float closestDist = 0;

		foreach (var magnet in magnets) {
			if (closest == null) {
				closest = magnet;
				closestDist = (magnet.MagnetPoint - rb.position).magnitude;
			} else {
				float _d = (magnet.MagnetPoint - rb.position).magnitude;
				if (closestDist > _d) {
					closest = magnet;
					closestDist = _d;
				}
			}
		}

		if (closest == null) {
			rb.gravityScale = 1;
			rb.velocity = Vector2.zero;
			_GroundCollider.enabled = true;
			isDragging = false;
			return;
		}

		rb.gravityScale = 0;

		float _dist = closestDist / closest.MagnetRadius;
		float _speed = Mathf.Lerp(20, 5f, _dist);

		float angle = Vector2.SignedAngle(Vector2.right, closest.MagnetPoint - rb.position);

		rb.velocity = Quaternion.Euler(0, 0, angle) * Vector2.right * _speed;
		clearOnNextFrame();
	}

	void clearOnNextFrame() {
		IEnumerator wait() {
			yield return new WaitForEndOfFrame();
			magnets.Clear();
		}
		StartCoroutine(wait());
	}
}
