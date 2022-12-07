using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    public AudioClip SpikeAttackAudioClip;

    private bool _isActivate = false;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _animator.SetBool("IsActivated", _isActivate);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") && !collision.CompareTag("Enemy"))
            return;

        if (!_isActivate)
        {
            AudioManager.Instance.SFXAudioSource.PlayOneShot(SpikeAttackAudioClip);
            
            if (collision.CompareTag("Player"))
                collision.GetComponent<PlayerStats>().DamageManager(1.0f * PlayerStats.Instance.TotalHealth.GetValue() / 4.0f);
            else if (collision.CompareTag("Enemy"))
                collision.GetComponent<EnemyStats>().UpdateHealth(-1.0f * collision.GetComponent<EnemyStats>().TotalEnemyHealth.GetValue() / 4.0f);
        }

        _isActivate = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") && !collision.CompareTag("Enemy"))
            return;

        _isActivate = false;
    }
}
