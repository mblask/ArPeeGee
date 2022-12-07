using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStats : MonoBehaviour
{
    public Stat TotalEnemyHealth;
    public Stat TotalEnemyMovementSpeed;
    public Stat TotalEnemyArmor;
    public Stat TotalEnemyDamage;
    public Stat TotalEnemyAttackRate;
    public Stat TotalEnemyCastingRate;
    public Stat TotalEnemyHealthRegenRate;

    private float _currentHealth;

    public float CurrentHealth
    {
        get
        {
            return _currentHealth;
        }
    }

    private EnemyController _enemyController;
    private EnemyData _enemyData;

    private void Start()
    {
        _currentHealth = TotalEnemyHealth.GetValue();
        _enemyController = GetComponent<EnemyController>();
        _enemyData = _enemyController.EnemyData;
    }

    public void DamageManager(float value)
    {
        float healthDamage = -value * (1.0f - TotalEnemyArmor.GetValue() / 100.0f);
        UpdateHealth(healthDamage);

        _enemyController.Animator.SetTrigger("Hit");
        _enemyController.BloodSplash();
        AudioManager.Instance.SFXAudioSource.PlayOneShot(_enemyData.HitClip);
    }

    public void UpdateHealth(float value)
    {
        _currentHealth += value;

        if (_currentHealth <= 0.0f)
        {
            _currentHealth = 0.0f;
            Die();
        }

        if (_enemyController.EnemyHealthPanel == null)
            return;

        _enemyController.EnemyHealthPanel.UpdateHealthPanel(_currentHealth / TotalEnemyHealth.GetValue());
    }

    private void Die()
    {
        _enemyController.IsDead = true;
        GameManager.Instance.UpdateEnemiesKilled();
        DropTreasure.Instance.DropItems(transform);
        DropTreasure.Instance.DropOneItem(_enemyController.DropSpecificItem, transform);
        PlayerStats.Instance.UpdateExperience(_enemyData.ExperienceValue);
        _enemyController.Animator.SetTrigger("Dead");
        _enemyController.Animator.SetBool("IsDead", _enemyController.IsDead);
        gameObject.layer = LayerMask.NameToLayer(ConstantContainer.BackgroundLayerString);
        Destroy(_enemyController.Rigidbody2d);

        if (!_enemyData.IsBoss)
            _enemyController.RemoveEnemyBody(_enemyController.RemoveBodyAfterTime);

        AudioManager.Instance.SFXAudioSource.PlayOneShot(_enemyData.DeathClip);

        if (_enemyData.IsBoss)
        {
            _enemyController.CheckIfSurroundingsEmpty = true;
        }
    }

    public void AddTemporaryModifier(Stat stat, float magnitude, float length)
    {
        if (!stat.IsTemporaryModified)
        {
            stat.SetTemporaryModified(true);
            StartCoroutine(TemporaryModifier(stat, magnitude, length));
        }
    }

    private IEnumerator TemporaryModifier(Stat stat, float magnitude, float length)
    {
        stat.AddModifier(magnitude);

        yield return new WaitForSeconds(length);

        stat.RemoveModifier(magnitude);
        stat.SetTemporaryModified(false);
    }
}
