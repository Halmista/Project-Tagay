using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HideAnimation
{
    Sink,
    FlickerDisappear
}

public class RevealZone : MonoBehaviour
{
    Dictionary<GameObject, Vector3> originalPositions = new();
    Dictionary<GameObject, Vector3> originalScales = new();

    [Header("Show when camera is open")]
    public GameObject[] revealObjects;

    [Header("Hide Animation")]
    public HideAnimation hideAnimation = HideAnimation.Sink;

    [Header("Hide when camera is open")]
    public GameObject[] hideObjects;

    bool playerInside;
    bool lastRevealState;
    bool lockReveal;

    public System.Action OnReveal;

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
        bool reveal = lockReveal || (playerInside && CameraManager.Instance.CameraOpen);

        if (reveal == lastRevealState)
            return;

        lastRevealState = reveal;
        if (reveal)
        {
            OnReveal?.Invoke();
        }

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

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer r in renderers)
                r.enabled = true;

            obj.transform.position = hidden;
            obj.transform.localScale = originalScales[obj] * 0.98f;

            obj.transform.DOMove(original, 0.45f)
                .SetEase(Ease.OutQuad);

            obj.transform.DOScale(originalScales[obj], 0.45f)
                .SetEase(Ease.OutQuad);
        }
        else
        {
            switch (hideAnimation)
            {
                case HideAnimation.Sink:
                    StartCoroutine(HideRoutine(obj, hidden));
                    break;

                case HideAnimation.FlickerDisappear:
                    StartCoroutine(FlickerDisappearRoutine(obj));
                    break;
            }
        }
    }

    IEnumerator HideRoutine(GameObject obj, Vector3 hidden)
    {
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

    IEnumerator FlickerDisappearRoutine(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);

        // Randomize so groups don't disappear simultaneously
        yield return new WaitForSeconds(Random.Range(0f, 0.25f));

        // Flicker 4 times
        for (int i = 0; i < 4; i++)
        {
            bool visible = (i % 2 == 0);

            foreach (Renderer r in renderers)
                r.enabled = visible;

            yield return new WaitForSeconds(Random.Range(0.04f, 0.08f));
        }

        // Final disappearance
        foreach (Renderer r in renderers)
            r.enabled = false;

        yield return new WaitForSeconds(0.05f);

        obj.SetActive(false);
    }

    public void LockReveal()
    {
        lockReveal = true;
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