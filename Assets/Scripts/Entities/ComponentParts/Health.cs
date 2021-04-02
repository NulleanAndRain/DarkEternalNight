using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public float MaxHealth;
    private float currHealth;
    public bool isImmortal;

    public bool canRegen;
    public float damageImmunityTime;
    private float lastDamageTime;

    public float regenAfterDamageCD;
    private float _regCD = 0;

    public float regenInterval;
    public float regenPerInterval;

    [Header("Knockback")]
    public float knockbackResist;
    private Rigidbody2D rb;
    public Vector2 center;

    [Header("Damage particles")]
    public ParticleSystem ParticlePrefab;
    public Vector2 ParticlesEmissionPoint;
    Vector3 _particlesPos;


    public Vector2 textParticlePos;
    public Vector2 textParticlePoint { get => (Vector2)transform.position + textParticlePos; }
    public GameObject textParticle { get => HUD.defaultTextParticle; }

    private bool isPlayer;

    private static class _colors {
        public static Color
            Heal        = new Color32(0x00, 0xF0, 0x1F, 0xFF),
            HitEnemy    = new Color32(0xFF, 0xFF, 0xFF, 0xFF),
            CritEnemy   = new Color32(0xFF, 0xCC, 0x44, 0xFF),
            HitPlayer   = new Color32(0xFF, 0xCC, 0xCC, 0xFF),
            CritPlayer  = new Color32(0xFF, 0x88, 0x88, 0xFF);
    }
    public Color healColor { get => _colors.Heal; }
    public Color damageColor { get => isPlayer ? _colors.HitPlayer : _colors.HitEnemy; }
    public Color critColor { get => isPlayer ? _colors.CritPlayer : _colors.CritEnemy; }

    public event Action onDowned = delegate { };
    public event Action<float, float> onHealthUpdate = delegate { };

    public bool isDowned { get; private set; } = false;

    // Start is called before the first frame update
    void Start() {
        isPlayer = gameObject.tag == "Player";
        currHealth = MaxHealth;
        _particlesPos = ParticlesEmissionPoint;

        rb = GetComponent<Rigidbody2D>();

        if (canRegen) StartCoroutine(Regen());
    }

    private IEnumerator Regen() {
        while (true) {
            if (isDowned) yield return new WaitForSeconds(GameManager.RespawnTime);
            if (_regCD == 0) {
                if (currHealth != MaxHealth) {
                    if (currHealth < MaxHealth) currHealth += regenPerInterval;
                    if (currHealth > MaxHealth) currHealth = MaxHealth;
                    onHealthUpdate(currHealth, MaxHealth);
                }
                yield return new WaitForSeconds(regenInterval);

            } else {
                yield return new WaitForSeconds(_regCD);
                _regCD = 0;
            }
        }
    }

    private Vector2 _lastDamageDir = new Vector2();

    Vector2 _kbDir = Vector2.zero;
    public void GetDamage(float amount, Vector2 pos, bool isCrit = false, float kbForce = 0) {
        if (Time.time - lastDamageTime < damageImmunityTime) return;
        lastDamageTime = Time.time;
        _regCD = regenAfterDamageCD;

        _lastDamageDir = (Vector2)transform.position + center - pos;

        float angle = Vector2.SignedAngle(Vector2.right, _lastDamageDir);

        if (Mathf.Abs(rb.velocity.x) >= 1e-4) _kbDir.x += -rb.velocity.x * 2;
        _kbDir.x = _lastDamageDir.x / _lastDamageDir.magnitude * 2;
        _kbDir.y = 1;

        kbForce *= (1 - knockbackResist) * rb.mass;
        if (kbForce < 0) kbForce = 0;
        _kbDir *= kbForce;
        rb.AddForce(_kbDir, ForceMode2D.Impulse);

        if (ParticlePrefab != null) {
            StartCoroutine(DamageParticles(Quaternion.Euler(angle, 90, 0)));
        }

        if (!isImmortal) {
            currHealth -= amount;
            if (currHealth <= 0) {
                onDowned();
                amount += currHealth;
                currHealth = 0;
                isDowned = true;
            }
        }

        DamageAmountPartice(amount, isCrit);
        onHealthUpdate(currHealth, MaxHealth);
    }

    public void Heal(float amount) {
        if (amount <= 0) return;
        currHealth += amount;
        if (currHealth > MaxHealth) {
            amount -= currHealth - MaxHealth;
            currHealth = MaxHealth;
        }

        onHealthUpdate(currHealth, MaxHealth);
        HealAmountParticle(amount);
    }

    private IEnumerator DamageParticles(Quaternion direction) {
        var _instance = Instantiate(ParticlePrefab, transform.position + _particlesPos, direction);
        yield return new WaitForSeconds(ParticlePrefab.main.duration);
        Destroy(_instance.gameObject);
    }

    public void Revive(float percent = 1) {
        isDowned = false;
        currHealth = MaxHealth * percent;
        onHealthUpdate(currHealth, MaxHealth);
    }

    private TextParticle _lastParticleDamage;
    private float _lastParticleTimeDamage;
    private float _lastParticleAmountDamage;
    private float _firstUpdateDamage;
    private void DamageAmountPartice(float amount, bool isCrit) {
        if (textParticle == null) return;
        if (_lastParticleDamage == null ||
            Time.time - _lastParticleTimeDamage > 1.5 ||
            Time.time - _firstUpdateDamage > 4 ||
            (_lastParticleDamage.worldPos - textParticlePoint).magnitude > 2
        ) {
            if (GlobalSettings.particlesStackingEnabled) {
                _lastParticleDamage = TextParticle.Instantiate(textParticle, textParticlePoint, HUD.floatToString(amount));
                _firstUpdateDamage = Time.time;
            } else {
                _lastParticleDamage = null;
            }

            if (isCrit) {
                _lastParticleDamage.mainColor = critColor;
            } else {
                _lastParticleDamage.mainColor = damageColor;
            }

            _lastParticleAmountDamage = amount;
        } else {
            _lastParticleAmountDamage += amount;
            _lastParticleDamage.updateTextColor(HUD.floatToString(_lastParticleAmountDamage), isCrit ? critColor : damageColor);
        }
        _lastParticleTimeDamage = Time.time;
    }

    private TextParticle _lastParticleHeal;
    private float _lastParticleTimeHeal;
    private float _lastParticleAmountHeal;
    private float _firstUpdateHeal;
    private void HealAmountParticle(float amount) {
        if (textParticle == null) return;
        if (_lastParticleHeal == null ||
            Time.time - _lastParticleTimeHeal > 1.5 ||
            Time.time - _firstUpdateHeal > 4 ||
            (_lastParticleHeal.worldPos - textParticlePoint).magnitude > 2
        ) {
            if (GlobalSettings.particlesStackingEnabled) {
                _lastParticleHeal = TextParticle.Instantiate(textParticle, textParticlePoint, HUD.floatToString(amount));
                _lastParticleHeal.mainColor = healColor;
                _firstUpdateHeal = Time.time;
            } else {
                _lastParticleHeal = null;
            }
            _lastParticleAmountHeal = amount;
        } else {
            _lastParticleAmountHeal += amount;
            _lastParticleHeal.updateText(HUD.floatToString(_lastParticleAmountHeal));
        }
        _lastParticleTimeHeal = Time.time;
    }

    public void setDamageImmunityForTime(float time) {
        lastDamageTime = Time.time;
        StartCoroutine(endImmunity(damageImmunityTime, time));
        damageImmunityTime = time;
    }

    private IEnumerator endImmunity(float prevImmVal, float time) {
        yield return new WaitForSeconds(time);
        damageImmunityTime = prevImmVal;
    }
}