using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSearcher : MonoBehaviour {

    public float SearchRadius;
    public Vector2 EyesPos;
    private Vector2 _eyePos { get => (Vector2)transform.position + EyesPos; }

    Vector2 _rc_dir = Vector2.zero;
    public GameObject playerInSight(out Vector2 playerPosCenter) {
        foreach (var c in Physics2D.OverlapCircleAll(_eyePos, SearchRadius)) {
            if (c.tag == "Player") {
                _rc_dir = (Vector2)c.bounds.center - _eyePos;
                Debug.DrawRay(_eyePos, _rc_dir, Color.red, Time.fixedDeltaTime);
                RaycastHit2D[] hitsC = Physics2D.RaycastAll(_eyePos, _rc_dir);
                playerPosCenter = c.bounds.center;
                foreach (var hit in hitsC) {
                    if (hit.collider.gameObject == gameObject || hit.collider.transform.parent?.gameObject == gameObject) continue;
                    if (hit.collider.tag == "Collectable" || hit.collider.tag == "Platforms" || hit.collider.tag == "ThrowableItem" || hit.collider.tag == tag) continue;
                    if (hit.collider.tag == "Player") return hit.collider.gameObject;
                    break;
                }

                _rc_dir.y += c.bounds.size.y / 2;
                Debug.DrawRay(_eyePos, _rc_dir, Color.red, Time.fixedDeltaTime);
                RaycastHit2D[] hitsT = Physics2D.RaycastAll(_eyePos, c.bounds.center + new Vector3(0, c.bounds.extents.y));
                foreach (var hit in hitsT) {
                    if (hit.collider.gameObject == gameObject || hit.collider.transform.parent?.gameObject == gameObject) continue;
                    if (hit.collider.tag == "Collectable" || hit.collider.tag == "Platforms" || hit.collider.tag == "ThrowableItem" || hit.collider.tag == tag) continue;
                    if (hit.collider.tag == "Player") return hit.collider.gameObject;
                    break;
                }

                _rc_dir.y -= c.bounds.size.y;
                Debug.DrawRay(_eyePos, _rc_dir, Color.red, Time.fixedDeltaTime);
                RaycastHit2D[] hitsB = Physics2D.RaycastAll(_eyePos, c.bounds.center - new Vector3(0, c.bounds.extents.y));
                foreach (var hit in hitsB) {
                    if (hit.collider.gameObject == gameObject || hit.collider.transform.parent?.gameObject == gameObject) continue;
                    if (hit.collider.tag == "Collectable" || hit.collider.tag == "Platforms" || hit.collider.tag == "ThrowableItem" || hit.collider.tag == tag) continue;
                    if (hit.collider.tag == "Player") return hit.collider.gameObject;
                    break;
                }
            }
        }
        playerPosCenter = Vector2.zero;
        return null;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_eyePos, SearchRadius);
    }
}
