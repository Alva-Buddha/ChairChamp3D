using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip chairGet;

    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer audioMixer;

    public static AudioManager Instance;// Make AudioManager a singleton which allows it to be accessed by any other script

    void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        #endregion
    }

    private void Start()
    {
        sfxSource.clip = chairGet;
        //sfxSource.Play();
    }

    // Play the actual audio clip
    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // Spawn the GameObject
        AudioSource audioSource = Instantiate(sfxSource, spawnTransform.position, Quaternion.identity);

        // Assign the audioClip
        audioSource.clip = audioClip;

        // Assign volume
        audioSource.volume = volume;

        // Play the sound
        audioSource.Play();

        // Get the length of the sound effect clip
        float clipLength = audioSource.clip.length;

        // Destroy the clip after it plays
        Destroy(audioSource.gameObject, clipLength);
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
