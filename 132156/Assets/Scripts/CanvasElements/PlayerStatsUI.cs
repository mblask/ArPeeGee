using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    public GameObject PlayerStatsGameObject;

    public Text HealthStat;
    public Text MagicStat;
    public Text ArmorStat;
    public Text MovementSpeedStat;
    public Text DamageStat;
    public Text AttackRateStat;
    public Text HealthRegenStat;
    public Text MagicRegenStat;

    private void Start()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.onPlayerStatsChange += UpdatePlayerStatsUI;

        PlayerStatsGameObject.SetActive(false);

        UpdatePlayerStatsUI();
    }

    private void Update()
    {
        UpdatePlayerStatsUI();

        if (Input.GetKeyDown(KeyCode.S))
        {
            PlayerStatsGameObject.SetActive(!PlayerStatsGameObject.activeSelf);
        }

        if (Input.GetButtonDown("Cancel") && PlayerStatsGameObject.activeSelf)
        {
            PlayerStatsGameObject.SetActive(false);
        }
    }

    public void UpdatePlayerStatsUI()
    {
        if (PlayerStats.Instance == null)
            return;

        HealthStat.text = PlayerStats.Instance.CurrentHealth.ToString("F1");
        MagicStat.text = PlayerStats.Instance.CurrentMagic.ToString("F1");
        ArmorStat.text = PlayerStats.Instance.TotalArmor.GetValue().ToString("F1");
        MovementSpeedStat.text = PlayerStats.Instance.TotalMovementSpeed.GetValue().ToString("F1");
        DamageStat.text = PlayerStats.Instance.TotalDamage.GetValue().ToString("F1");
        AttackRateStat.text = PlayerStats.Instance.TotalAttackRate.GetValue().ToString("F1");
        HealthRegenStat.text = PlayerStats.Instance.TotalHealthRegenRate.GetValue().ToString("F1");
        MagicRegenStat.text = PlayerStats.Instance.TotalMagicRegenRate.GetValue().ToString("F1");
    }
}
