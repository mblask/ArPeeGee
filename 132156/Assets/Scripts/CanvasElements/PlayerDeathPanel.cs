using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathPanel : MonoBehaviour
{
    public GameObject GameOverPanel;

    void Start()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.onPlayerDeath += ActivatePlayerDeathPanel;
    }

    public void ActivatePlayerDeathPanel()
    {
        GameOverPanel.SetActive(true);

        float timePassed = GameManager.Instance.TimePassedInTheGame;

        if (timePassed < 60.0f)
            Debug.Log(timePassed.ToString("F1") + " s");
        else if (timePassed > 60.0f)
        {
            float minutes = Mathf.Floor(timePassed / 60.0f);
            float seconds = timePassed - minutes * 60.0f;
            Debug.Log(minutes.ToString("F0") + " min and " + seconds.ToString("F0") + " seconds");
        }
        else
        {
            float hours = Mathf.Floor(timePassed / 3600.0f);
            float minutes = Mathf.Floor((timePassed - hours * 3600.0f) / 60.0f);
            float seconds = timePassed - hours * 3600.0f - minutes * 60.0f;
            Debug.Log(hours.ToString("F0") + " hours, " + minutes.ToString("F1") + " minutes and " + seconds.ToString("F0") + " seconds");
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(ConstantContainer.MainMenuName);
    }
}
