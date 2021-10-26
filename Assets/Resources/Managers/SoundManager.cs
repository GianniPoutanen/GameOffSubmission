using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    public enum eSound
    {
        LightArmourGrassStep,
        SwordHeavySwipe,
        SwordHeavyContact
    }

    public enum eSoundType
    {
        Music,
        WeaponSoundEffect,
        FootSoundEffect
    }

    private static Dictionary<eSound, int> soundIndents = new Dictionary<eSound, int>();



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
        GameObject soundGameObject = GameAssets.Instance.GetObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.volume = GameManager.Instance.pieceSoundEffectsVolume;
        audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Plays a given sound clip
    /// </summary>
    /// <param name="clip"> The clip </param>
    public static void PlaySound(AudioClip clip, eSoundType type)
    {
        GameObject soundGameObject = GameAssets.Instance.GetObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();

        float volume = 1;
        if (type == eSoundType.FootSoundEffect)
        {
            volume = GameManager.Instance.pieceStepsEffectsVolume;
        }
        else if (type == eSoundType.Music)
        {
            volume = GameManager.Instance.musicVolume;
        }
        else if (type == eSoundType.WeaponSoundEffect)
        {
            volume = GameManager.Instance.pieceSoundEffectsVolume;
        }

        audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Resets
    /// </summary>
    /// <param name="sound"></param>
    /// <returns></returns>
    public static AudioClip GetNextSound(eSound sound)
    {
        if (!soundIndents.ContainsKey(sound))
        {
            soundIndents.Add(sound, 0);
        }
        GameAssets ga = Resources.Load<GameAssets>("Managers");
        foreach (GameAssets.SoundAudioClip clips in GameAssets.Instance.soundAudioClips)
        {
            if (clips.sound == sound)
            {
                AudioClip clip = clips.Clip(soundIndents[sound]);
                soundIndents[sound] = (soundIndents[sound] + 1) % clips.Count;

                return clip;
            }
        }
        Debug.LogError("No sound of type " + sound + " found. Try checking the GameAssets object");
        return null;
    }
}
