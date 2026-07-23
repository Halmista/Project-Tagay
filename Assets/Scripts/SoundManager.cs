using System;
using System.Collections;
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
    private Coroutine fadeRoutine;

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
            // Don't restart if it's already playing the same clip
            if (ambienceSource.clip == clip && ambienceSource.isPlaying) return;

            ambienceSource.clip = clip;
            ambienceSource.loop = true;
            ambienceSource.volume = 1f;
            ambienceSource.Play();
        }
    }

    public void StopAmbience()
    {
        if (ambienceSource != null)
        {
            ambienceSource.Stop();
            ambienceSource.clip = null;
        }
    }

    // Optional: Smoothly fade ambience in or out
    public void FadeAmbience(string id, float targetVolume, float duration)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeAmbienceRoutine(id, targetVolume, duration));
    }

    private IEnumerator FadeAmbienceRoutine(string id, float targetVolume, float duration)
    {
        if (targetVolume > 0 && soundLookup.TryGetValue(id, out AudioClip clip))
        {
            if (ambienceSource.clip != clip)
            {
                ambienceSource.clip = clip;
                ambienceSource.loop = true;
                if (!ambienceSource.isPlaying) ambienceSource.Play();
            }
        }

        float startVolume = ambienceSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            ambienceSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        ambienceSource.volume = targetVolume;

        if (targetVolume <= 0f)
        {
            StopAmbience();
        }
    }
}