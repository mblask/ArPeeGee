using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerVictoryPanel : MonoBehaviour
{
    public GameObject VictoryPanel;
    public Text EnemiesKilledText;
    public Text TimeText;

    private const string MainMenuName = "MainMenu";

    private void Start()
    {
        GameManager.Instance.onPlayerVictory += ActivatePlayerDeathPanel;
    }

    public void ActivatePlayerDeathPanel()
    {
        VictoryPanel.SetActive(true);
        EnemiesKilledText.text = GameManager.Instance.EnemiesKilled.ToString();
        //Debug.Log(GameManager.Instance.EnemiesKilled);

        float timePassed = GameManager.Instance.TimePassedInTheGame;

        if (timePassed < 60.0f)
        {
            TimeText.text = timePassed.ToString("F1") + " s";
            //Debug.Log(timePassed.ToString("F1") + " s");
        }
        else if (timePassed > 60.0f)
        {
            float minutes = Mathf.Floor(timePassed / 60.0f);
            float seconds = timePassed - minutes * 60.0f;

            TimeText.text = minutes.ToString("F0") + " min and " + seconds.ToString("F0") + " seconds";
            //Debug.Log(minutes.ToString("F0") + " min and " + seconds.ToString("F0") + " seconds");
        }
        else
        {
            float hours = Mathf.Floor(timePassed / 3600.0f);
            float minutes = Mathf.Floor((timePassed - hours * 3600.0f) / 60.0f);
            float seconds = timePassed - hours * 3600.0f - minutes * 60.0f;

            TimeText.text = hours.ToString("F0") + " hours, " + minutes.ToString("F1") + " minutes and " + seconds.ToString("F0") + " seconds";
            //Debug.Log(hours.ToString("F0") + " hours, " + minutes.ToString("F1") + " minutes and " + seconds.ToString("F0") + " seconds");
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(MainMenuName);
    }
}
