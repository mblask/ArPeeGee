using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : Interactable
{
    [Header("Door Properties")]
    public bool IsLocked = false;
    public bool LoadsNewLevel = false;

    [Header("Door Assets")]
    public Sprite DoorClosed;
    public Sprite DoorOpen;
    public AudioClip DoorOpenAudioClip;
    public AudioClip DoorCloseAudioClip;
    public AudioClip DoorLockedAudioClip;

    private bool _isClosed = true;
    private const string SandboxLevelName = "Sandbox";
    private const string DoorName = "Door";

    public override void Interact()
    {
        if (IsLocked)
        {
            AudioManager.Instance.SFXAudioSource.PlayOneShot(DoorLockedAudioClip);
        }
        else
        {
            if (_isClosed)
            {
                OpenDoor();
                _isClosed = false;
            }
            else
            {
                CloseDoor();
                _isClosed = true;
            }
        }

        OnDefocus();
        Invoke("ResetInteraction", 0.2f);
    }

    public void OpenDoor()
    {
        GetComponent<SpriteRenderer>().sprite = DoorOpen;
        GetComponent<BoxCollider2D>().enabled = false;
        AudioManager.Instance.SFXAudioSource.PlayOneShot(DoorOpenAudioClip);

        if (LoadsNewLevel)
        {
            //Save current game state

            if (SceneManager.GetActiveScene().name == SandboxLevelName)
                SceneManager.LoadScene(1);

            GameManager.Instance.SavePlayerData();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void CloseDoor()
    {
        GetComponent<SpriteRenderer>().sprite = DoorClosed;
        GetComponent<BoxCollider2D>().enabled = true;
        AudioManager.Instance.SFXAudioSource.PlayOneShot(DoorCloseAudioClip);
    }

    private void OnMouseOver()
    {
        if (gameObject != null)
        {
            GameManager.Instance.ShowItemNamePanel(DoorName, transform);
        }
    }

    private void OnMouseExit()
    {
        GameManager.Instance.HideItemNamePanel();
    }
}
