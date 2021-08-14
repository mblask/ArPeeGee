using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Larry : MonoBehaviour
{
    public List<AudioClip> LarryAudioClips;
    
    [Space]
    public float LarrysNextSpeechInterval = 3.0f;

    private bool _LarryHasSpoken = false;

    private float _timer = 0.0f;

    private void Update()
    {
        if (_LarryHasSpoken)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0.0f)
                _LarryHasSpoken = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (!_LarryHasSpoken)
        {
            _LarryHasSpoken = true;
            _timer += LarrysNextSpeechInterval;

            AudioManager.Instance.SFXAudioSource.PlayOneShot(LarryAudioClips[Random.Range(0,LarryAudioClips.Count)]);
        }
    }
}
