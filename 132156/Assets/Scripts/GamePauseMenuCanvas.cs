using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GamePauseMenuCanvas : MonoBehaviour
{
    public GameObject GamePauseMenuPanel;
    public GameObject AudioSettingsPanel;
    public Canvas GamePlayCanvas;

    private bool _menuActive = false;

    private GameObject _lastActive = null;
    private GameObject _currentlyActive = null;

    private const string MainMenuSceneName = "MainMenu";
    private const string AudioSettingsPanelName = "AudioSettingsPanel";

    private void Start()
    {
        AudioManager.Instance.LoadAudioSettings();

        GamePauseMenuPanel.SetActive(_menuActive);
        AudioSettingsPanel.SetActive(_menuActive);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PlayerStats.Instance != null && PlayerStats.Instance.IsDead)
                return;

            if (_currentlyActive == null || _currentlyActive == GamePauseMenuPanel)
            {
                _menuActive = !_menuActive;

                Time.timeScale = _menuActive ? 0.0f : 1.0f;

                GamePauseMenuPanel.SetActive(_menuActive);

                _currentlyActive = GamePauseMenuPanel;
            }
            else if (_currentlyActive != null)
            {
                DeactivatePanel(_currentlyActive);

                ActivatePanel(_currentlyActive);
            }
        }
    }

    public void SaveAndQuitGame()
    {
        //PlayerPrefs save game data
        //When game saving complete go to MainMenuScene
        GameManager.Instance.SavePlayerData();

        SceneManager.LoadScene(MainMenuSceneName);
    }

    public void ActivatePanel(GameObject panel)
    {
        _lastActive = _currentlyActive;

        _lastActive.SetActive(false);

        panel?.SetActive(true);

        _currentlyActive = panel;
    }

    public void DeactivatePanel(GameObject panel)
    {
        _currentlyActive = _lastActive;

        if (panel.name == AudioSettingsPanelName)
        {
            AudioManager.Instance.SaveAudioSettings();
        }

        panel?.SetActive(false);

        _lastActive = panel;
    }
}
