using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Main Audio Manager References")]
    public AudioMixer MainAudioMixer;
    public AudioSource MusicAudioSource;
    public AudioSource SFXAudioSource;

    public Slider MusicVolumeSlider;
    public Slider SFXVolumeSlider;
    private const string MusicVolumeParameter = "MusicVolume";
    private const string SFXVolumeParameter = "SFXVolume";

    [Header("Background Music Clips")]
    public AudioClip MainBackgroundMusic;
    public AudioClip FinalBossAudioClip;

    [Header("Player Audio Clips")]
    public AudioClip WalkingAudioClip;
    public AudioClip WeaponSwingAudioClip;
    public AudioClip WeaponHitAudioClip;
    public AudioClip LevelUpAudioClip;
    public List<AudioClip> PlayerHitAudioClips = new List<AudioClip>();
    public AudioClip PlayerDeathAudioClip;

    [Header("Other Audio Clips")]
    public AudioClip ItemPickupAudioClip;

    private float _noSoundThreshold = -30.0f;
    private float _noSoundVolume = -80.0f;

    public float MusicVolume
    {
        get
        {
            MainAudioMixer.GetFloat(MusicVolumeParameter, out float value);
            return value;
        }

        set
        {
            if (value <= _noSoundThreshold)
                value = _noSoundVolume;

            MainAudioMixer.SetFloat(MusicVolumeParameter, value);
        }
    }

    public float SFXVolume
    {
        get
        {
            MainAudioMixer.GetFloat(SFXVolumeParameter, out float value);
            return value;
        }

        set
        {
            if (value <= _noSoundThreshold)
                value = _noSoundVolume;

            MainAudioMixer.SetFloat(SFXVolumeParameter, value);
        }
    }

    private float _defaultMusicVolume;
    private float _defaultSFXVolume;

    #region Singleton
    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;
    }
    #endregion

    private void Start()
    {
        SetLevelBackgroundMusic();

        MainAudioMixer.GetFloat(MusicVolumeParameter, out float musicVolume);
        MainAudioMixer.GetFloat(SFXVolumeParameter, out float sfxVolume);

        MusicVolumeSlider.value = musicVolume;
        SFXVolumeSlider.value = sfxVolume;

        _defaultMusicVolume = musicVolume;
        _defaultSFXVolume = sfxVolume;
    }

    public void SetLevelBackgroundMusic()
    {
        if (SceneManager.GetActiveScene().buildIndex < 5)
        {
            MusicAudioSource.clip = MainBackgroundMusic;
        }
        else
        {
            MusicAudioSource.clip = FinalBossAudioClip;
        }

        MusicAudioSource.Play();
    }

    public void SaveAudioSettings() 
    {
        PlayerPrefs.SetFloat(MusicVolumeParameter, MusicVolume);
        PlayerPrefs.SetFloat(SFXVolumeParameter, SFXVolume);
    }

    public void LoadAudioSettings()
    {
        MusicVolume = PlayerPrefs.GetFloat(MusicVolumeParameter, _defaultMusicVolume);
        SFXVolume = PlayerPrefs.GetFloat(SFXVolumeParameter, _defaultSFXVolume);
    }
}
