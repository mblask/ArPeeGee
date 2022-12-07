using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprite : MonoBehaviour
{
    public AudioClip SteppingAudioClip;

    public void PlayOnStep()
    {
        if (SteppingAudioClip != null && AudioManager.Instance != null)
            AudioManager.Instance.SFXAudioSource.PlayOneShot(SteppingAudioClip);
    }
}
