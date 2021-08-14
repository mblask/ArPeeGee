using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Journal : MonoBehaviour
{
    public GameObject JournalPanel;
    public AudioClip JournalOpen;
    public AudioClip JournalClose;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            JournalPanel.SetActive(!JournalPanel.activeSelf);

            if (JournalPanel.activeSelf)
            {
                AudioManager.Instance.SFXAudioSource.PlayOneShot(JournalOpen);
            }
            else
            {
                AudioManager.Instance.SFXAudioSource.PlayOneShot(JournalClose);
            }
        }
    }
}
