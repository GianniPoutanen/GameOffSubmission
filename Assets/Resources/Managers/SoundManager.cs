using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{

    public enum eSoundType
    {
        Music,
        SoundEffct
    }

    /// <summary>
    /// Plays a given music clip on a loop
    /// </summary>
    /// <param name="clip"> The clip </param>
    public static void PlayMusic(AudioClip clip)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.volume = GameManager.Instance.musicVolume;
        audioSource.clip = clip;
        audioSource.Play();
    }

    /// <summary>
    /// Plays a given sound clip
    /// </summary>
    /// <param name="clip"> The clip </param>
    public static void PlaySound(AudioClip clip)
    {
        GameObject soundGameObject = GameAssets.Instance.GetActiveObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.volume = GameManager.Instance.soundEffectVolume;
        audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Plays a given sound clip
    /// </summary>
    /// <param name="clip"> The clip </param>
    public static void PlaySound(AudioClip clip, eSoundType type)
    {
        GameObject soundGameObject = GameAssets.Instance.GetActiveObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();

        float volume = 1;
        if (type == eSoundType.SoundEffct)
        {
            volume = GameManager.Instance.soundEffectVolume;
        }
        else if (type == eSoundType.Music)
        {
            volume = GameManager.Instance.musicVolume;
        }

        audioSource.PlayOneShot(clip);
    }
}
