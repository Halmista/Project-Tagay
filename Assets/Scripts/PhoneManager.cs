using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneManager : MonoBehaviour
{
    public Sprite junBackground;
    public Sprite unknownBackground;
    public GameObject phoneUI;
    public static PhoneManager Instance;
    public Transform content;          // The Content object in the Scroll View
    public GameObject messagePrefab;   // Your Message prefab
    public ScrollRect scrollRect;

    public KeyCode toggleKey = KeyCode.Tab;

    void Start()
    {
        phoneUI.SetActive(false);
        
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            phoneUI.SetActive(!phoneUI.activeSelf);
            SoundManager.Instance.PlaySFX("PhoneGrab");

            if (phoneUI.activeSelf)
            {
                NotificationManager.Instance.HideNotification();

                // Complete the "Open Phone" objective
                if (ObjectiveManager.Instance.IsCurrentObjective("OpenPhone"))
                {
                    ObjectiveManager.Instance.CompleteObjective();
                }

                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }
    }

    public void ReceiveMessage(string message)
    {
        SoundManager.Instance.PlaySFX("PhoneBuzz");
        SoundManager.Instance.PlaySFX("SMS");

        Debug.Log("Received: " + message);

        GameObject msg = Instantiate(messagePrefab, content);

        TMP_Text text = msg.GetComponentInChildren<TMP_Text>();
        text.text = message;

        LayoutElement layout = msg.GetComponent<LayoutElement>();

        Canvas.ForceUpdateCanvases();

        float height = Mathf.Max(10, text.preferredHeight + 10);
        layout.preferredHeight = height;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public void HidePhone()
    {
        SoundManager.Instance.PlaySFX("PhoneHide");
        phoneUI.SetActive(false);
    }
}