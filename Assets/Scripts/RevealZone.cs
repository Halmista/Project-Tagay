using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealZone : MonoBehaviour
{
    Dictionary<GameObject, Vector3> originalPositions = new();
    Dictionary<GameObject, Vector3> originalScales = new();

    [Header("Show when camera is open")]
    public GameObject[] revealObjects;

    [Header("Hide when camera is open")]
    public GameObject[] hideObjects;

    bool playerInside;
    bool lastRevealState;

    void Start()
    {
        foreach (GameObject obj in hideObjects)
        {
            if (obj != null)
            {
                originalPositions[obj] = obj.transform.position;
                originalScales[obj] = obj.transform.localScale;
            }
        }

        foreach (GameObject obj in revealObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    void Update()
    {
        bool reveal = playerInside && CameraManager.Instance.CameraOpen;

        if (reveal == lastRevealState)
            return;

        lastRevealState = reveal;

        foreach (GameObject obj in revealObjects)
        {
            if (obj != null)
                FadeObject(obj, reveal);
        }

        foreach (GameObject obj in hideObjects)
        {
            if (obj != null)
                AnimateHideObject(obj, !reveal);
        }
    }
    void AnimateHideObject(GameObject obj, bool visible)
    {
        obj.transform.DOKill();

        Vector3 original = originalPositions[obj];
        Vector3 hidden = original + Vector3.down * 1f;

        if (visible)
        {
            obj.SetActive(true);

            obj.transform.position = hidden;
            obj.transform.localScale = originalScales[obj] * 0.98f;

            obj.transform.DOMove(original, 0.45f)
                .SetEase(Ease.OutQuad);

            obj.transform.DOScale(originalScales[obj], 0.45f)
                .SetEase(Ease.OutQuad);
        }
        else
        {
            StartCoroutine(HideRoutine(obj, hidden));
        }
    }

    IEnumerator HideRoutine(GameObject obj, Vector3 hidden)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        // Flicker 3 times
        for (int i = 0; i < 3; i++)
        {
            foreach (Renderer r in renderers)
                r.enabled = false;

            yield return new WaitForSeconds(0.04f);

            foreach (Renderer r in renderers)
                r.enabled = true;

            yield return new WaitForSeconds(0.04f);
        }

        // Sink into the ground
        Sequence seq = DOTween.Sequence();

        seq.Join(
            obj.transform.DOMove(hidden, 0.45f)
                .SetEase(Ease.InQuad));

        seq.Join(
            obj.transform.DOScale(originalScales[obj] * 0.98f, 0.45f)
                .SetEase(Ease.InQuad));

        yield return seq.WaitForCompletion();

        obj.SetActive(false);
    }
    void FadeObject(GameObject obj, bool visible)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            foreach (Material mat in r.materials)
            {
                Color c = mat.color;

                // Make sure the object is active before fading in
                if (visible)
                    obj.SetActive(true);

                mat.DOKill();

                mat.DOColor(
                    new Color(c.r, c.g, c.b, visible ? 1f : 0f),
                    0.5f)
                .OnComplete(() =>
                {
                    if (!visible)
                        obj.SetActive(false);
                }
                );
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}