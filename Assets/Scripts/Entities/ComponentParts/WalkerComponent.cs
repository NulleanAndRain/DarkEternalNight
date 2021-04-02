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
    public CircleCollider2D groundTrigger;

    public float moveSpeed;
    public float jumpForce;
    public float moveSmoothing;
    public float maxClimbAngle;
    public bool stepingDown { get; private set; } = false;
    public bool onGround { get => isOnGround();}

    public Vector2 boundsTopLeft;
    public Vector2 boundsBotRight;

    private event Action _onEndSteppingDown = delegate { };
    public event Action onStepUp = delegate { };
    public event Action onHorizontalBump = delegate { };

    Vector2 _vel = Vector2.zero;
    Vector2 _target_vel = Vector2.zero;
    private bool _isSelfMoving = false;

    ContactFilter2D filter = new ContactFilter2D();

    private void Start () {
        filter.layerMask = _LayerMaskGroundPlatforms;
    }

    public void moveDir(float dir) {
        if (!canMove())
            return;
        _target_vel.x = dir * moveSpeed;
        _target_vel.y = rb.velocity.y;

        if (Mathf.Abs(dir) >= 1e-4) {
            _isSelfMoving = true;
            rb.velocity = Vector2.SmoothDamp(rb.velocity, _target_vel, ref _vel, moveSmoothing);
        } else if (_isSelfMoving) {
            _isSelfMoving = false;
            Vector2 temp = rb.velocity;
            temp.x = 0;
            rb.velocity = temp;
        }
    }

    public void jump() {
        jumpWithForce(jumpForce);
    }
    public void jumpHalfHeight() {
        jumpWithForce(jumpForce * 0.75f);
    }
    public void jumpWithForce(float force) {
        if (!onGround || !canMove()) return;
        rb.AddForce(new Vector2(0, force), ForceMode2D.Impulse);
    }

    List<ContactPoint2D> contactPoints = new List<ContactPoint2D>();
    public bool isOnGround () {
        var colls = Physics2D.OverlapCircleAll(groundTrigger.bounds.center, groundTrigger.radius, _LayerMaskGroundPlatforms);
        foreach (var c in colls) {
            if (c.gameObject != gameObject && c.attachedRigidbody != null) {
                if (c.attachedRigidbody.isKinematic) {
                    return Math.Abs(rb.velocity.y) <= 1e-4;
                }
                return Mathf.Abs(rb.velocity.y - c.attachedRigidbody.velocity.y) <= 1e-4;
            }
        }
        return false;
    }

    private bool canMove () {
        var colls = Physics2D.OverlapCircleAll(groundTrigger.bounds.center, groundTrigger.radius, _LayerMaskGroundPlatforms);
        foreach (var c in colls) {
            if (c.GetContacts(contactPoints) > 0) {
				var point = contactPoints[0];
                float angle = Vector2.Angle(Vector2.up, point.normal) % 180;
				if (angle <= maxClimbAngle)
                    return true;
            } else {
				return true;
            }
        }
        return colls.Length == 0;
    }

    Vector2 _plOverlap1;
    Vector2 _plOverlap2;
    public void stepDownPlatform() {
        if (_onEndSteppingDown == null) _onEndSteppingDown = delegate { };
        stepingDown = true;

        _plOverlap1.x = rb.position.x + boundsTopLeft.x * .95f;
        _plOverlap1.y = rb.position.y + boundsBotRight.y * .99f;

        _plOverlap2.x = rb.position.x + boundsBotRight.x * .95f;
        _plOverlap2.y = rb.position.y + boundsBotRight.y * 1.01f;

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
}
