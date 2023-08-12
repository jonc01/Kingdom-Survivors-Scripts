using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Settings")]
    [SerializeField] public bool toggleSFX;
    [SerializeField] public bool toggleMusic;
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource, sfxSource;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        //Play audio clip, restart/stop audio clip if already playing
        if(sfxSource.isPlaying) sfxSource.Stop();
        sfxSource.PlayOneShot(clip);
    }

    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value; //Not currenty in-use, setting Music/Effects separately
    }

    public void ChangeMusicVolume(float value)
    {
        musicSource.volume = value;
    }

    public void ChangeSFXVolume(float value)
    {
        sfxSource.volume = value;
    }

    public void ToggleSFX(bool toggle)
    {
        sfxSource.mute = toggle;
    }

    public void ToggleMusic(bool toggle)
    {
        musicSource.mute = toggle;
    }
}
