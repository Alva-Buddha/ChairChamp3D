using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("Audio Clips - Music")]
    public AudioClip titleMusic;
    public AudioClip preRoundMusic;
    public AudioClip levelThemeMusic;
    public AudioClip roundEndMusic;
    public AudioClip playerSoloWinMusic;

    [Header("Audio Clips - SFX")]
    public AudioClip powerupDash;
    public AudioClip powerupStun;
    public AudioClip powerupPull;
    public AudioClip powerupSwap;
    public AudioClip chairGet;
    public AudioClip playerSoloWinSFX;
    public AudioClip roundEndSFX;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        PlayMusicClip(titleMusic, true);
    }

    // Play selected music clip
    public void PlayMusicClip(AudioClip musicClip, bool isLooping)
    {
        musicSource.Stop();
        musicSource.clip = musicClip;
        musicSource.loop = isLooping;
        musicSource.Play();
    }

    // Stop current music clip
    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Pauses current music clip
    public void PauseMusic()
    {
        musicSource.Pause();
    }

    // Unpauses current music clip
    public void UnpauseMusic()
    {
        musicSource.UnPause();
    }

    // Play selected sound effect clip
    public void PlaySFXClip(AudioClip sfxClip)
    {
        sfxSource.clip = sfxClip;
        sfxSource.Play();
    }

    // Play the pre round music
    public void PlayPreRoundMusic()
    {
        musicSource.Stop();
        musicSource.clip = preRoundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    // Function to adjust the master volume
    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
    }

    // Function to adjust the music volume
    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
    }

    // Function to adjust the sound effects volume
    public void SetSFXVolume(float level)
    {
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(level) * 20f);
    }
}
