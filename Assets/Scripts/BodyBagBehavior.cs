using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BodyBagBehaviour : MonoBehaviour
{
    bool squirming;
    public ParticleSystem smoke;

    [Header("Audio Settings")]
    [Tooltip("The ID defined in your SoundManager Sound Library for the garbage ruffle sound")]
    public string ruffleSoundId = "GarbageRuffle";

    public void Squirm()
    {
        if (squirming)
            return;

        StartCoroutine(SquirmRoutine());
    }

    IEnumerator SquirmRoutine()
    {
        squirming = true;

        // ---------- Stage 1 : Small twitch ----------
        // Play subtle SFX for the small twitch
        if (SoundManager.Instance != null && !string.IsNullOrEmpty(ruffleSoundId))
            SoundManager.Instance.PlaySFX(ruffleSoundId);

        Sequence stage1 = DOTween.Sequence();

        stage1.Append(
            transform.DOPunchRotation(
                new Vector3(0, 0, 5),
                0.18f,
                4,
                0.3f));

        stage1.Join(
            transform.DOPunchPosition(
                new Vector3(0.015f, 0f, 0f),
                0.18f,
                3,
                0.3f));

        yield return stage1.WaitForCompletion();

        // Let the player wonder if they imagined it.
        yield return new WaitForSeconds(0.8f);

        if (smoke != null)
            smoke.Play();

        // ---------- Stage 2 : Violent struggle ----------
        // Play SFX again for the intense struggle
        if (SoundManager.Instance != null && !string.IsNullOrEmpty(ruffleSoundId))
            SoundManager.Instance.PlaySFX(ruffleSoundId);

        Sequence stage2 = DOTween.Sequence();

        stage2.Append(
            transform.DOShakeRotation(
                1f,
                new Vector3(0, 0, 20),
                25,
                90));

        stage2.Join(
            transform.DOShakePosition(
                1f,
                0.08f,
                25));

        stage2.Join(
            transform.DOPunchScale(
                Vector3.one * 0.12f,
                0.6f,
                6,
                0.6f));

        yield return stage2.WaitForCompletion();

        squirming = false;
    }
}