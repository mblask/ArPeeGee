using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button ContinueGameButton;

    private GameObject _currentlyActive = null;
    private PlayerData _continueGameData;

    private const string AudioSettingsPanelName = "AudioSettingsPanel";

    void Start()
    {

        AudioManager.Instance.LoadAudioSettings();

        if (!CheckForSavedGames())
        {
            ContinueGameButton.interactable = false;
        }
        else
        {
            ContinueGameButton.interactable = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_currentlyActive == null)
            {
                QuitGame();
            }
            else
            {
                DeactivateMainMenuPanel(_currentlyActive);
            }
        }
    }

    public void ActivateMainMenuPanel(GameObject panel)
    {
        if (panel == null)
            return;

        panel.SetActive(true);

        _currentlyActive = panel;
    }

    public void DeactivateMainMenuPanel(GameObject panel)
    {
        if (panel == null)
            return;

        if (panel.name == AudioSettingsPanelName)
        {
            AudioManager.Instance.SaveAudioSettings();
        }

        panel.SetActive(false);

        _currentlyActive = null;
    }

    public bool CheckForSavedGames()
    {
        //Code for checking if a game was played in the past
        _continueGameData = SaveSystem.LoadPlayer();

        if (_continueGameData == null)
            return false;
        else
            return true;
    }

    public void StartNewGame(GameObject playerPrefab)
    {
        SaveSystem.SavePlayer(playerPrefab, null, null, null, null, null, null);

        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {
        Debug.Log(_continueGameData.CurrentLevel);
        SceneManager.LoadScene(_continueGameData.CurrentLevel);
    }

    public void QuitGame()
    {
        Debug.Log("Exit the program");
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
