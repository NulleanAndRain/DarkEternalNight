using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class WalkerComponent : MonoBehaviour {

    public static LayerMask _LayerMaskGround { get => LayerMask.GetMask("Default", "Ground"); }
    public static LayerMask _LayerMaskGroundPlatforms { get => LayerMask.GetMask("Default", "Ground", "Platforms"); }

    public Rigidbody2D rb;
    public Collider2D[] colliders;

    public float moveSpeed;
    public float jumpForce;
    public float moveSmoothing;
    private bool stepingDown = false;

    public Vector2 boundsTopLeft;
    public Vector2 boundsBotRight;
    public float stepCheckDistance;
    public bool canStepOnBlock;

    private event Action _onEndSteppingDown = delegate { };
    public event Action onStepUp = delegate { };
    public event Action onHorizontalBump = delegate { };

    Vector2 _vel = Vector2.zero;
    Vector2 _target_vel = Vector2.zero;
    private bool _isSelfMoving = false;
    public void moveDir(int dir) {
        _target_vel.x = dir * moveSpeed;
        _target_vel.y = rb.velocity.y;

        if (dir != 0) {
            _isSelfMoving = true;
            rb.velocity = Vector2.SmoothDamp(rb.velocity, _target_vel, ref _vel, moveSmoothing);
        } else if (_isSelfMoving) {
            _isSelfMoving = false;
            Vector2 temp = rb.velocity;
            temp.x = 0;
            rb.velocity = temp;
        }

        if (canStepOnBlock && !stepingDown && canStepUp(dir)) {
            rb.MovePosition(rb.position + Vector2.up * 1.05f);
            onStepUp();
        }
    }

    public void jump() {
        jumpWithForce(jumpForce);
    }

    public void jumpHalfHeight() {
        jumpWithForce(jumpForce * 0.75f);
    }

    public void jumpWithForce(float force) {
        rb.AddForce(new Vector2(0, force), ForceMode2D.Impulse);
    }

    private Vector2 _overlap1 = new Vector2();
    private Vector2 _overlap2 = new Vector2();
    public bool canStepUp(int dir) {
        if (dir == 0 || !isOnGround()) return false;
        bool isBotStepable = false;
        bool isBotPlatform = false;
        _overlap1.y = rb.position.y + boundsBotRight.y + 0.99f;
        _overlap2.y = rb.position.y + boundsBotRight.y + 0.2f;

        _overlap1.x = rb.position.x + dir * (boundsBotRight.x + 0.01f);
        _overlap2.x = rb.position.x + dir * (boundsBotRight.x + stepCheckDistance);

        //Debug.DrawLine(_overlap1, _overlap2, Color.red, Time.fixedDeltaTime);

        foreach (var down in Physics2D.OverlapAreaAll(_overlap1, _overlap2, _LayerMaskGroundPlatforms)) {
            if (down.gameObject != gameObject) {
                if (down.transform.parent != null) if (down.transform.parent.gameObject == gameObject) continue;
                if (down.tag == "Enemy" || down.tag == "Collectable" || down.tag == "ThrowableItem" || down.tag == "Player")
                    continue;
                //Debug.DrawLine(down.bounds.min, down.bounds.max, Color.magenta, Time.fixedDeltaTime);
                isBotStepable = true;
                isBotPlatform = down.tag == "Platforms";
                break;
            }
        }
        if (!isBotStepable) return false;

        _overlap1.y = rb.position.y + boundsTopLeft.y + 1.1f;
        _overlap2.y = rb.position.y + boundsBotRight.y + 1.2f;

        //Debug.DrawLine(_overlap1, _overlap2, Color.red, Time.fixedDeltaTime);

        foreach (var up in Physics2D.OverlapAreaAll(_overlap1, _overlap2)) {
            if (up.gameObject != gameObject) {
                if (up.transform.parent != null) if (up.transform.parent.gameObject == gameObject) continue;
                if (isBotPlatform && up.tag != "Platforms") return false;
                if (up.tag == "Enemy" || up.tag == "Collectable" || up.tag == "ThrowableItem" || up.tag == "Player")
                    continue;
                onHorizontalBump();
                return false;
            }
        }

        return true;
    }
    
    public bool isOnGround() {
        _overlap1.y = rb.position.y - 0.1f;
        _overlap2.y = rb.position.y + 0.1f;

        _overlap1.x = rb.position.x + boundsTopLeft.x;
        _overlap2.x = rb.position.x + boundsBotRight.x;

        foreach (var c in Physics2D.OverlapAreaAll(_overlap1, _overlap2, _LayerMaskGroundPlatforms)) {
            if (c.gameObject != gameObject && c.attachedRigidbody != null) {
                if (c.attachedRigidbody.isKinematic)
                    return Math.Abs(rb.velocity.y) <= 1e-4;
                return Mathf.Abs(rb.velocity.y - c.attachedRigidbody.velocity.y) <= 1e-4;
            }
        }
        return false;
    }


    public bool isStandingInPlatform() {
        foreach (var c in Physics2D.OverlapAreaAll(
                new Vector2(rb.position.x - 0.49f, rb.position.y + 0.99f),
                new Vector2(rb.position.x + 0.49f, rb.position.y + 0.1f))
            ) {
            if (c.gameObject != gameObject && c.tag == "Platforms")
                return true;
        }
        return false;
    }


    Vector2 _plOverlap1;
    Vector2 _plOverlap2;
    public void stepDownPlatform() {
        if (_onEndSteppingDown == null) _onEndSteppingDown = delegate { };
        stepingDown = true;

        _plOverlap1.x = rb.position.x - 0.49f;
        _plOverlap1.y = rb.position.y;

        _plOverlap2.x = rb.position.x + 0.49f;
        _plOverlap2.y = rb.position.y - 0.1f;

        foreach (var c in Physics2D.OverlapAreaAll(
               _plOverlap1,
               _plOverlap2,
               _LayerMaskGroundPlatforms)
            ) {
            if (c.gameObject != gameObject) {
                if (c.tag != "Platforms") return;
                if (!Physics2D.GetIgnoreCollision(colliders[0], c)) {
                    foreach(var e in colliders) Physics2D.IgnoreCollision(e, c, true);

                    void unignore() {
                        foreach (var e in colliders) Physics2D.IgnoreCollision(e, c, false);
                    }

                    _onEndSteppingDown += unignore;
                }

            }
        }
    }

    public void endStepDown() {
        if (_onEndSteppingDown != null) {
            _onEndSteppingDown();
            _onEndSteppingDown = null;
        }
        stepingDown = false;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        _overlap1 = (Vector2)transform.position + boundsTopLeft;
        _overlap2.x = transform.position.x + boundsTopLeft.x;
        _overlap2.y = transform.position.y + boundsBotRight.y;
        Gizmos.DrawRay(_overlap1, _overlap2 - _overlap1);
        _overlap1.y = transform.position.y + boundsBotRight.y;
        _overlap2.x = transform.position.x + boundsBotRight.x;
        Gizmos.DrawRay(_overlap1, _overlap2 - _overlap1);
        _overlap1.x = transform.position.x + boundsBotRight.x;
        _overlap2.y = transform.position.y + boundsTopLeft.y;
        Gizmos.DrawRay(_overlap1, _overlap2 - _overlap1);
        _overlap1.y = transform.position.y + boundsTopLeft.y;
        _overlap2 = (Vector2)transform.position + boundsTopLeft;
        Gizmos.DrawRay(_overlap1, _overlap2 - _overlap1);
    }
}
