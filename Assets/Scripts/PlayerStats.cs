using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public Stat TotalHealth;
    public Stat TotalMagic;
    public Stat TotalMovementSpeed;
    public Stat TotalArmor;
    public Stat TotalDamage;
    public Stat TotalAttackRate;
    public Stat TotalCastingRate;
    public Stat TotalHealthRegenRate;
    public Stat TotalMagicRegenRate;

    [Header("Read-only")]
    [SerializeField] private int _currentLevel = 1;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _currentMagic;
    [SerializeField] private float _healthIncrementOnLevel;
    [SerializeField] private float _magicIncrementOnLevel;
    [SerializeField] private float _currentExperience = 0.0f;
    [SerializeField] private float _experienceNeededToLevel = 100.0f;
    [SerializeField] private float _experienceIncrement = 100.0f;
    [SerializeField] private bool _isDead = false;
    public int CurrentLevel
    {
        get
        {
            return _currentLevel;
        }

        set
        {
            _currentLevel = value;
        }
    }
    public float CurrentHealth
    {
        get
        {
            return _currentHealth;
        }

        set
        {
            _currentHealth = value;
        }
    }
    public float CurrentMagic
    {
        get
        {
            return _currentMagic;
        }

        set
        {
            _currentMagic = value;
        }
    }
    public float CurrentExperience
    {
        get
        {
            return _currentExperience;
        }

        set
        {
            _currentExperience = value;
        }
    }
    public float ExperienceNeededToLevel => _experienceNeededToLevel;
    public float ExperienceIncrement => _experienceIncrement;
    public bool IsDead => _isDead;

    private PlayerController _playerController;
    private Animator _playerAnimator;
    private const string UntaggedTag = "Untagged";

    private const string HealthPanelName = "PlayerHealthPanel";
    private const string MagicPanelName = "PlayerMagicPanel";
    private GameObject _playerHealthPanel;
    private GameObject _playerMagicPanel;
    private Slider _playerHealthSlider;
    private Text _playerHealthText;
    private Slider _playerMagicSlider;
    private Text _playerMagicText;

    public delegate void OnPlayerStatsChange();
    public OnPlayerStatsChange onPlayerStatsChange;

    public delegate void OnPlayerDeath();
    public OnPlayerDeath onPlayerDeath;

    #region Singleton
    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;
    }
    #endregion

    private void Start()
    {
        _playerAnimator = GetComponentInChildren<Animator>();
        _playerController = GetComponent<PlayerController>();

        _playerHealthPanel = GameObject.Find(HealthPanelName);
        _playerMagicPanel = GameObject.Find(MagicPanelName);

        _playerHealthSlider = _playerHealthPanel.GetComponentInChildren<Slider>();
        _playerHealthText = _playerHealthPanel.GetComponentInChildren<Text>();
        _playerMagicSlider = _playerMagicPanel.GetComponentInChildren<Slider>();
        _playerMagicText = _playerMagicPanel.GetComponentInChildren<Text>();

        UpdateHealth(TotalHealth.GetValue());
        UpdateMagic(TotalMagic.GetValue());

        EquipmentManager.Instance.onEquipmentChanged += UpdatePlayerStats;
    }

    private void Update()
    {
        ModifyEssenceOverTime(Essence.Health, TotalHealthRegenRate.GetValue());
        ModifyEssenceOverTime(Essence.Magic, TotalMagicRegenRate.GetValue());
    }

    public void DamageManager(float value)
    {
        float healthDamage = -value * (1.0f - TotalArmor.GetValue() / 100.0f);
        UpdateHealth(healthDamage);
        _playerController.BloodSplash();
        _playerAnimator.SetTrigger("Hit");
        int randomAudioClipIndex = Random.Range(0, AudioManager.Instance.PlayerHitAudioClips.Count - 1);
        AudioManager.Instance.SFXAudioSource.PlayOneShot(AudioManager.Instance.PlayerHitAudioClips[randomAudioClipIndex]);
    }

    public void UpdateHealth(float value)
    {
        _currentHealth += value;

        if (_currentHealth > TotalHealth.GetValue())
            _currentHealth = TotalHealth.GetValue();

        if (_currentHealth <= 0.0f)
        {
            _currentHealth = 0.0f;
            Die();
        }

        UpdateHealthSlider();
    }

    public void UpdateMagic(float value)
    {
        _currentMagic += value;

        if (_currentMagic > TotalMagic.GetValue())
            _currentMagic = TotalMagic.GetValue();

        if (_currentMagic <= 0.0f)
            _currentMagic = 0.0f;

        UpdateMagicSlider();
    }

    public void ModifyEssenceOverTime(Essence essence, float regenRate)
    {
        if (regenRate == 0.0f)
            return;

        switch (essence)
        {
            case Essence.Health:
                if (regenRate > 0.0f && _currentHealth == TotalHealth.GetValue())
                    return;
                UpdateHealth(regenRate * Time.deltaTime);
                break;

            case Essence.Magic:
                if (regenRate > 0.0f && _currentMagic == TotalMagic.GetValue())
                    return;
                UpdateMagic(regenRate * Time.deltaTime);
                break;
        }
    }

    private void Die()
    {
        GameManager.Instance.SavePlayerData();

        _isDead = true;
        gameObject.tag = UntaggedTag;
        _playerAnimator.SetTrigger("Dead");
        _playerAnimator.SetBool("IsDead", _isDead);
        AudioManager.Instance.SFXAudioSource.PlayOneShot(AudioManager.Instance.PlayerDeathAudioClip);

        if (onPlayerDeath != null)
            onPlayerDeath.Invoke();
    }

    public void UpdateExperience(float experience)
    {
        _currentExperience += experience;

        if (_currentExperience >= _experienceNeededToLevel)
        {
            UpdateLevel();
        }

        if (onPlayerStatsChange != null)
            onPlayerStatsChange.Invoke();
    }

    public void UpdateToLevelAndExperience(int level, float experience)
    {
        if (level == 1 && experience == 0.0f)
            return;

        _currentLevel = level;

        for (int i = 0; i < level-1; i++)
        {
            UpdateExperienceRequiredToLevel();
        }

        float totalHealthIncrement = _healthIncrementOnLevel * (_currentLevel - 1);
        float totalMagicIncrement = _magicIncrementOnLevel * (_currentLevel - 1);

        TotalHealth.AddModifier(totalHealthIncrement);
        TotalMagic.AddModifier(totalMagicIncrement);

        _currentExperience = experience;

        if (onPlayerStatsChange != null)
            onPlayerStatsChange.Invoke();
    }

    private void UpdateLevel()
    {
        float totalHealthIncrement = 0.0f;
        float totalMagicIncrement = 0.0f;

        if (_currentLevel > 1)
        {
            totalHealthIncrement = _healthIncrementOnLevel * (_currentLevel - 1);
            totalMagicIncrement = _magicIncrementOnLevel * (_currentLevel - 1);

            TotalHealth.RemoveModifier(totalHealthIncrement);
            TotalMagic.RemoveModifier(totalMagicIncrement);
        }

        _currentLevel += 1;
        UpdateExperienceRequiredToLevel();

        totalHealthIncrement = _healthIncrementOnLevel * (_currentLevel - 1);
        totalMagicIncrement = _magicIncrementOnLevel * (_currentLevel - 1);

        TotalHealth.AddModifier(totalHealthIncrement);
        TotalMagic.AddModifier(totalMagicIncrement);

        _currentHealth = TotalHealth.GetValue();
        _currentMagic = TotalMagic.GetValue();

        UpdateHealthSlider();
        UpdateMagicSlider();

        AudioManager.Instance.SFXAudioSource.PlayOneShot(AudioManager.Instance.LevelUpAudioClip);
    }

    private void UpdateExperienceRequiredToLevel()
    {
        _experienceIncrement *= 1.0f + 4.076f * Mathf.Exp(-_currentLevel * 0.356f);
        _experienceNeededToLevel += _experienceIncrement;
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

    public void UpdateHealthSlider()
    {
        _playerHealthSlider.maxValue = TotalHealth.GetValue();
        _playerHealthSlider.value = _currentHealth;
        _playerHealthText.text = _currentHealth.ToString("F0") + "/" + TotalHealth.GetValue().ToString("F0");
    }

    public void UpdateMagicSlider()
    {
        _playerMagicSlider.maxValue = TotalMagic.GetValue();
        _playerMagicSlider.value = _currentMagic;
        _playerMagicText.text = _currentMagic.ToString("F0") + "/" + TotalMagic.GetValue().ToString("F0");
    }

    public void UpdatePlayerStats(Equipment newItem, Equipment oldItem)
    {
        if (newItem != null)
        {
            TotalHealth.AddModifier(newItem.HealthModifier);
            UpdateHealthSlider();
            TotalMagic.AddModifier(newItem.MagicModifier);
            UpdateMagicSlider();
            TotalMovementSpeed.AddModifier(newItem.MovementSpeedModifier);
            TotalArmor.AddModifier(newItem.ArmorModifier);
            TotalDamage.AddModifier(newItem.DamageModifier);
            TotalAttackRate.AddModifier(newItem.AttackRateModifier);
            TotalCastingRate.AddModifier(newItem.CastingRateModifier);
            TotalHealthRegenRate.AddModifier(newItem.HealthRegenModifier);
            TotalMagicRegenRate.AddModifier(newItem.MagicRegenModifier);
        }

        if (oldItem != null)
        {
            TotalHealth.RemoveModifier(oldItem.HealthModifier);
            UpdateHealthSlider();
            TotalMagic.RemoveModifier(oldItem.MagicModifier);
            UpdateMagicSlider();
            TotalMovementSpeed.RemoveModifier(oldItem.MovementSpeedModifier);
            TotalArmor.RemoveModifier(oldItem.ArmorModifier);
            TotalDamage.RemoveModifier(oldItem.DamageModifier);
            TotalAttackRate.RemoveModifier(oldItem.AttackRateModifier);
            TotalCastingRate.RemoveModifier(oldItem.CastingRateModifier);
            TotalHealthRegenRate.RemoveModifier(oldItem.HealthRegenModifier);
            TotalMagicRegenRate.RemoveModifier(oldItem.MagicRegenModifier);
        }

        if (onPlayerStatsChange != null)
            onPlayerStatsChange.Invoke();
    }
}
