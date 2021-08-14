using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "ArPeeGee/Enemy", order = 10)]
public class EnemyData : ScriptableObject
{
    [Header("Main Enemy Settings")]
    public string EnemyName;
    public bool IsBoss;
    public bool IsRangedAndMeele;
    public bool IsRanged;
    public float FollowRadius; //FollowRadius > AttackRadius!!
    public bool CanCharge;
    public int Armor;
    public Color BloodSplashColor;
    public ParticleSystem BloodSplashPrefab;
    public float ExperienceValue;

    [Header("Audio Clips")]
    public AudioClip StepAudioClip;
    public AudioClip HitClip;
    public AudioClip DeathClip;
    public AudioClip BossSeesPlayerAudioClip;

    [Header("Enemy Attack")]
    public float AttackRadius;
    public List<GameObject> EnemySpells;

    [Header("Charge Settings")]
    public float ChargeRate;
    public float ChargeRadius;
    public float ChargeCooldown;
    public float ChargeSpeedBoost;
    public float ChargeDamage;
    public AudioClip ChargeAudioClip;
}
