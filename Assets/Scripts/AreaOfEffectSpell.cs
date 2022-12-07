using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffectSpell : Spell
{
    [Header("Main AOE settings")]
    public float RadiusOfEffect;
    public ParticleSystem AOEParticleSystem;
    public LayerMask TargetsLayerMask;

    [Header("Audio")]
    public AudioClip AreaOfEffectSpellAudioClip;

    [Header("Read-only")]
    [SerializeField] private float _iceSpellSlowDown = -2.0f;
    [SerializeField] private float _slowDownDuration = 1.0f;
    [SerializeField] private float _aoeDamageModifier = 1.0f;
    [SerializeField] private string _casterTag = "";

    private Transform _areaOfEffectOrigin;

    public void DealAreaDamage()
    {
        if (_areaOfEffectOrigin == null)
            _areaOfEffectOrigin = transform;

        Instantiate(AOEParticleSystem, _areaOfEffectOrigin.position, AOEParticleSystem.transform.rotation);
        AudioManager.Instance.SFXAudioSource.PlayOneShot(AreaOfEffectSpellAudioClip);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_areaOfEffectOrigin.position, RadiusOfEffect, TargetsLayerMask);
        
        if (colliders.Length != 0)
        {
            foreach (Collider2D collider in colliders)
            {                
                if (collider.tag == _casterTag || collider.isTrigger)
                    continue;
                
                if (_casterTag == "Enemy")
                {
                    PlayerStats playerStats = collider.GetComponent<PlayerStats>();
                    if (playerStats != null)
                    {
                        if (SpellElement == SpellElement.Ice)
                        {
                            playerStats.AddTemporaryModifier(PlayerStats.Instance.TotalMovementSpeed, _iceSpellSlowDown, _slowDownDuration);
                        }
                        playerStats.DamageManager(BaseSpellDamage * _aoeDamageModifier);
                    }
                }
                else if (_casterTag == "Player")
                {
                    EnemyStats enemyStats = collider.GetComponent<EnemyStats>();
                    if (enemyStats != null)
                    {
                        if (SpellElement == SpellElement.Ice)
                        {
                            enemyStats.AddTemporaryModifier(enemyStats.TotalEnemyMovementSpeed, _iceSpellSlowDown/4, _slowDownDuration);
                        }
                        enemyStats.DamageManager(BaseSpellDamage * _aoeDamageModifier);
                    }
                }
                else
                {
                    Debug.LogWarning("Unknown LayerMask or Object with Tag hit");
                }
            }
        }
    }

    public void SetOriginPosition(Transform origin)
    {
        if (origin != null)
            _areaOfEffectOrigin = origin;
    }

    public void SetCasterTag(string tag)
    {
        if (tag == "" || tag == "Untagged")
            return;

        _casterTag = tag;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, RadiusOfEffect);
    }
}
