using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Spell
{
    [Header("Main projectile settings")]
    public float MovementSpeed = 4.0f;
    public float RadiusOfExplosionEffect = 0.5f;

    [Header("Particle System")]
    public ParticleSystem Explosion;

    [Header("Raycast settings")]
    public LayerMask Obstacles;
    public float RaycastDistance = 0.01f;

    [Header("Audio")]
    public AudioClip ProjectileCastingAudioClip;
    public AudioClip ProjectileDestructionAudioClip;

    [Header("Read-only")]
    [SerializeField] private float _iceSpellSlowDown = -2.0f;
    [SerializeField] private float _slowDownDuration = 1.0f;
    [SerializeField] private float _lifeTime = 2.0f;

    private Color _fireExplosionColor = new Color(255.0f, 54.0f, 0.0f);
    private Color _iceExplosionColor = new Color(0.0f, 131.0f, 255.0f);
    private string _casterTag;
    private float _projectileDamageModifier = 1.0f;
    private float _stopwatch = 0.0f;

    private void Start()
    {
        AudioManager.Instance.SFXAudioSource.PlayOneShot(ProjectileCastingAudioClip);

        ParticleSystem.MainModule settings = Explosion.main;

        switch (SpellElement)
        {
            case SpellElement.Fire:
                settings.startColor = _fireExplosionColor;
                break;
            case SpellElement.Ice:
                settings.startColor = _iceExplosionColor;
                break;
        }
    }

    private void Update()
    {
        if (_stopwatch >= _lifeTime)
            DestroyObject();

        _stopwatch += Time.deltaTime;

        MoveProjectile();

        CheckHitsAndTargets();
    }

    private void CheckHitsAndTargets()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.right, RaycastDistance, Obstacles);
        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.tag == _casterTag)
                return;

            if (hitInfo.collider.tag == "Player")
            {
                PlayerStats playerStats = hitInfo.collider.GetComponent<PlayerStats>();
                if (SpellElement == SpellElement.Ice)
                {
                    playerStats.AddTemporaryModifier(PlayerStats.Instance.TotalMovementSpeed, _iceSpellSlowDown, _slowDownDuration);
                }

                playerStats.DamageManager(BaseSpellDamage * _projectileDamageModifier);
            }
            else if (hitInfo.collider.tag == "Enemy")
            {
                if (SpellElement == SpellElement.Ice)
                {
                    EnemyStats enemyStats = hitInfo.collider.GetComponent<EnemyStats>();
                    enemyStats.AddTemporaryModifier(enemyStats.TotalEnemyMovementSpeed, _iceSpellSlowDown / 4, _slowDownDuration);
                }

                Collider2D[] colliders = Physics2D.OverlapCircleAll(hitInfo.collider.transform.position, RadiusOfExplosionEffect, Obstacles);
                if (colliders.Length != 0)
                {
                    foreach (Collider2D target in colliders)
                    {
                        if (target.tag == _casterTag || target.isTrigger)
                            continue;

                        target.GetComponent<EnemyStats>().DamageManager(BaseSpellDamage * _projectileDamageModifier);
                    }
                }
            }

            DestroyObject();
        }
    }

    public void SetCasterTag(string tag)
    {
        if (tag == "" || tag == "Untagged")
            return;

        _casterTag = tag;
    }

    private void MoveProjectile()
    {
        if (MovementSpeed == 0.0f)
            return;

        Vector3 increment = transform.right * MovementSpeed * Time.deltaTime;
        transform.position += increment;
    }

    private void DestroyObject()
    {
        ParticleSystem explosionClone = Instantiate(Explosion, transform.position, Quaternion.identity);
        explosionClone.transform.SetParent(null);
        explosionClone.Play();
        AudioManager.Instance.SFXAudioSource.PlayOneShot(ProjectileDestructionAudioClip);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * RaycastDistance);
    }
}
