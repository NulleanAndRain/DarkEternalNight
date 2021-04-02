using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour {
	public float DissolveDuration;
	public Vector2 DissolveTransform;

	public Collider2D trigger;

	public event Action<Collider2D> onCollect = delegate { };
	public event Action onDissolveStart = delegate { };

	public bool collectableOnlyByPlayer;

	void OnTriggerEnter2D(Collider2D other) {
		if (collectableOnlyByPlayer && other.tag != "Player") return;
		onCollect(other);
	}

	public void collect() {
		if (DissolveDuration > 0) {
			StartCoroutine(dissolve());
		} else {
			Destroy(gameObject);
		}
	}

	private IEnumerator dissolve() {
		trigger.enabled = false;
		float _time = 0;
		onDissolveStart();

		bool isMoving = DissolveTransform.magnitude > 0;

		Vector2 newPos = DissolveTransform;
		if (isMoving) {
			newPos.x += transform.position.x;
			newPos.y += transform.position.y;
		}
		Renderer _r = GetComponent<Renderer>();

		float oldA = _r.material.color.a;
		var _c = new Color(_r.material.color.r, _r.material.color.g, _r.material.color.b, oldA);

		Vector2 oldPos = new Vector2();
		oldPos.x = transform.position.x;
		oldPos.y = transform.position.y;

		while (_time < DissolveDuration) {
			float dt = _time / DissolveDuration;

			if (isMoving) transform.position = Vector2.Lerp(oldPos, newPos, dt);
			_c.a = Mathf.Lerp(oldA, 0, dt);
			_r.material.color = _c;

			_time += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		Destroy(gameObject);
	}
}
