using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    public Slider ExperienceSlider;
    public Text Level;

    private PlayerStats _playerStats;

    private void Start()
    {
        _playerStats = PlayerStats.Instance;

        UpdateExperienceBar();

        _playerStats.onPlayerStatsChange += UpdateExperienceBar;
    }

    public void UpdateExperienceBar()
    {
        Level.text = _playerStats.CurrentLevel.ToString();

        ExperienceSlider.minValue = _playerStats.ExperienceNeededToLevel - _playerStats.ExperienceIncrement;
        ExperienceSlider.maxValue = _playerStats.ExperienceNeededToLevel;
        ExperienceSlider.value = _playerStats.CurrentExperience;
    }
}
