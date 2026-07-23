using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AmbientZone : MonoBehaviour
{
    [Header("Sound Settings")]
    [Tooltip("The ID defined in your SoundManager Sound Library")]
    public string ambientSoundId;
    public float fadeDuration = 1.0f;

    private void Awake()
    {
        // Ensure the collider is set up as a trigger
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && SoundManager.Instance != null)
        {
            // Smoothly fade in the zone sound
            SoundManager.Instance.FadeAmbience(ambientSoundId, 1f, fadeDuration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && SoundManager.Instance != null)
        {
            // Smoothly fade out the sound when leaving
            SoundManager.Instance.FadeAmbience(ambientSoundId, 0f, fadeDuration);
        }
    }

    private void OnDisable()
    {
        // If the zone object gets deactivated while playing, stop/fade the sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.FadeAmbience(ambientSoundId, 0f, fadeDuration);
        }
    }
}