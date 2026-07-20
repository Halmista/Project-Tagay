using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public enum NotificationSender
{
    Jun,
    Unknown
}
public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [Header("UI")]
    public RectTransform junPanel;
    public RectTransform unknownPanel;
    public TMP_Text junMessageText;
    public TMP_Text unknownMessageText;

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

        junPanel.anchoredPosition = hiddenPosition;
        unknownPanel.anchoredPosition = hiddenPosition;
    }

    public void ShowNotification(string message, NotificationSender sender)
    {
        Debug.Log($"Sender: {sender}");

        Debug.Log($"Jun Panel: {junPanel}");
        Debug.Log($"Unknown Panel: {unknownPanel}");
        RectTransform panel;
        TMP_Text text;

        if (sender == NotificationSender.Jun)
        {
            panel = junPanel;
            text = junMessageText;

            unknownPanel.anchoredPosition = hiddenPosition;
        }
        else
        {
            panel = unknownPanel;
            text = unknownMessageText;

            junPanel.anchoredPosition = hiddenPosition;
        }

        text.text = message;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        panel.DOKill();


        panel
            .DOAnchorPos(visiblePosition, slideDuration)
            .SetEase(Ease.OutCubic);

        currentRoutine = StartCoroutine(HideRoutine(panel));
    }

    IEnumerator HideRoutine(RectTransform panel)
    {
        yield return new WaitForSeconds(displayTime);

        panel
            .DOAnchorPos(hiddenPosition, slideDuration)
            .SetEase(Ease.InCubic);

        currentRoutine = null;
    }

    public void HideNotification()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        junPanel.DOKill();
        unknownPanel.DOKill();

        junPanel
            .DOAnchorPos(hiddenPosition, slideDuration)
            .SetEase(Ease.InCubic);

        unknownPanel
            .DOAnchorPos(hiddenPosition, slideDuration)
            .SetEase(Ease.InCubic);

        currentRoutine = null;
    }
}