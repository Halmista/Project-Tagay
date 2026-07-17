using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [Header("UI")]
    public RectTransform notificationPanel;
    public TMP_Text messageText;

    [Header("Animation")]
    public float displayTime = 3f;
    public float slideDuration = 0.4f;

    public Vector2 hiddenPosition;
    public Vector2 visiblePosition;

    Coroutine currentRoutine;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        notificationPanel.anchoredPosition = hiddenPosition;
    }

    public void ShowNotification(string message)
    {
        messageText.text = message;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        notificationPanel.DOKill();

        notificationPanel
            .DOAnchorPos(visiblePosition, slideDuration)
            .SetEase(Ease.OutCubic);

        currentRoutine = StartCoroutine(HideRoutine());
    }

    IEnumerator HideRoutine()
    {
        yield return new WaitForSeconds(displayTime);

        notificationPanel
            .DOAnchorPos(hiddenPosition, slideDuration)
            .SetEase(Ease.InCubic);

        currentRoutine = null;
    }

    public void HideNotification()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        notificationPanel.DOKill();

        notificationPanel
            .DOAnchorPos(hiddenPosition, slideDuration)
            .SetEase(Ease.InCubic);

        currentRoutine = null;
    }
}