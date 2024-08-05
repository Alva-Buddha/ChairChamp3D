using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

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
    public AudioClip powerupCollected;
    public AudioClip powerupDash;
    public AudioClip powerupStun;
    public AudioClip powerupPull;
    public AudioClip powerupSwap;
    public AudioClip chairGet;
    public AudioClip playerSoloWinSFX;
    public AudioClip roundEndSFX;

    public AudioClip hazardPuddleSFX;
    public AudioClip hazardMeteoriteSFX;
    public AudioClip hazardOctopusSFX;
    public AudioClip hazardBlackHoleSFX;
    public AudioClip hazardBlackHoleFormingSFX;
    public AudioClip movementSwimmingSFX;
    public AudioClip movementRunningSFX;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    public static AudioManager instance;

    public void Awake()
    {
        // used to implement singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            PlayMusicClip(titleMusic, true);
        }
        
    }

    // Play the pre round music
    public void PlayPreRoundMusic()
    {
        PlayMusicClip(preRoundMusic, true);
    }

    // Play level theme music
    public void PlayLevelThemeMusic()
    {
        PlayMusicClip(levelThemeMusic, true);
    }

    // Play round end music
    public void PlayRoundEndAudio()
    {
        PlaySFXClip(roundEndSFX);
        PlayMusicClip(roundEndMusic, true);
    }

    // Play round end music if the player won vs all AI
    public void PlayPlayerSoloWinAudio()
    {
        PlaySFXClip(playerSoloWinSFX);
        PlayMusicClip(playerSoloWinMusic, true);
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

    // Play sound effect clip for collecting a powerup
    public void PlayPowerupCollectedSFX()
    {
        PlaySFXClip(powerupCollected);
    }

    // Play sound effect clip for the Pull powerup
    public void PlayPullSFX()
    {
        PlaySFXClip(powerupPull);
    }

    // Play sound effect clip for the Dash powerup
    public void PlayDashSFX()
    {
        PlaySFXClip(powerupDash);
    }

    // Play sound effect clip for the Stun powerup
    public void PlayStunSFX()
    {
        PlaySFXClip(powerupStun);
    }

    // Play sound effect clip for the Swap powerup
    public void PlaySwapSFX()
    {
        PlaySFXClip(powerupSwap);
    }

    // Play sound effect clip for the Puddle hazard
    public void PlayPuddleSFX()
    {
        PlaySFXClip(hazardPuddleSFX);
    }
    // Play sound effect clip for the Meteorite hazard
    public void PlayMeteoriteSFX()
    {
        PlaySFXClip(hazardMeteoriteSFX);
    }
    // Play sound effect clip for the Octopus hazard
    public void PlayOctopusSFX()
    {
        PlaySFXClip(hazardOctopusSFX);
    }
    // Play sound effect clip for the Black Hole hazard
    public void PlayBlackHoleSFX()
    {
        PlaySFXClip(hazardBlackHoleSFX);
    }
    // Play sound effect clip for the Black Hole hazard forming
    public void PlayBlackHoleFormingSFX()
    {
        PlaySFXClip(hazardBlackHoleFormingSFX);
    }
    // Play sound effect clip for the running animation
    public void PlayRunningSFX()
    {
        PlaySFXClip(movementRunningSFX);
    }
    // Play sound effect clip for the swimming animation
    public void PlaySwimmingSFX()
    {
        PlaySFXClip(movementSwimmingSFX);
    }

    // Play selected sound effect clip
    public void PlaySFXClip(AudioClip sfxClip)
    {
        sfxSource.clip = sfxClip;
        sfxSource.loop = false;
        sfxSource.Play();
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
