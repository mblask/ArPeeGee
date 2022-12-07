using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fountain : MonoBehaviour
{
    public FountainType Type;
    public ParticleSystem FountainParticleSystemPrefab;
    public AudioClip RegenerationAudioClip;

    private float _fountainRegenRate = 0.0f;
    private ParticleSystem _fountainParticleSystem;
    [Header ("Read-only")]
    [SerializeField] private float _healthRegenDivider = 20.0f;
    [SerializeField] private float _magicRegenDivider = 10.0f;
    [SerializeField] private Color _healthColor = Color.red;
    [SerializeField] private Color _magicColor = Color.blue;

    private void Start()
    {
        AudioManager.Instance.SFXAudioSource.clip = RegenerationAudioClip;
        AudioManager.Instance.SFXAudioSource.loop = true;
    }

    private void Update()
    {
        if (_fountainParticleSystem != null && !_fountainParticleSystem.IsAlive())
            Destroy(_fountainParticleSystem);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || collision.isTrigger)
            return;

        StartRegenerationOn(collision.GetComponent<PlayerStats>());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || collision.isTrigger)
            return;

        StopRegenerationOn(collision.GetComponent<PlayerStats>());
    }


    public void StartRegenerationOn(PlayerStats target)
    {
        if (_fountainParticleSystem == null)
        {
            _fountainParticleSystem = Instantiate(FountainParticleSystemPrefab, (Vector2)transform.position + Vector2.down, Quaternion.identity);
            _fountainParticleSystem.transform.SetParent(transform);
        }

        ParticleSystem.MainModule settings = _fountainParticleSystem.main;

        switch (Type)
        {
            case FountainType.Health:
                _fountainRegenRate = target.TotalHealth.GetValue() / _healthRegenDivider;
                target.TotalHealthRegenRate.AddModifier(_fountainRegenRate);
                settings.startColor = _healthColor;
                break;
            case FountainType.Magic:
                _fountainRegenRate = target.TotalMagic.GetValue() / _magicRegenDivider;
                target.TotalMagicRegenRate.AddModifier(_fountainRegenRate);
                settings.startColor = _magicColor;
                break;
            default:
                break;
        }

        AudioManager.Instance.SFXAudioSource.Play();
        _fountainParticleSystem.Play();
    }

    public void StopRegenerationOn(PlayerStats target)
    {
        switch (Type)
        {
            case FountainType.Health:
                target.TotalHealthRegenRate.RemoveModifier(_fountainRegenRate);
                break;
            case FountainType.Magic:
                target.TotalMagicRegenRate.RemoveModifier(_fountainRegenRate);
                break;
            default:
                break;
        }

        AudioManager.Instance.SFXAudioSource.Stop();
        _fountainParticleSystem.Stop();
    }
}
