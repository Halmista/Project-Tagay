using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundEntry
{
    public string id;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioSource ambienceSource;
    //public AudioSource voiceSource;

    [Header("Sound Library")]
    public List<SoundEntry> sounds = new();

    Dictionary<string, AudioClip> soundLookup;

    void Awake()
    {
        Instance = this;

        soundLookup = new Dictionary<string, AudioClip>();

        foreach (SoundEntry sound in sounds)
        {
            if (!soundLookup.ContainsKey(sound.id))
                soundLookup.Add(sound.id, sound.clip);
        }
    }

    public void PlaySFX(string id)
    {
        if (soundLookup.TryGetValue(id, out AudioClip clip))
            sfxSource.PlayOneShot(clip);
    }

    /*public void PlayVoice(string id)
    {
        if (soundLookup.TryGetValue(id, out AudioClip clip))
            voiceSource.PlayOneShot(clip);
    }*/

    public void PlayMusic(string id)
    {
        if (soundLookup.TryGetValue(id, out AudioClip clip))
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void PlayAmbience(string id)
    {
        if (soundLookup.TryGetValue(id, out AudioClip clip))
        {
            ambienceSource.clip = clip;
            ambienceSource.Play();
        }
    }
}