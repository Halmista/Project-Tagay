using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraManager : MonoBehaviour
{
    public LayerMask photoLayer;
    public static CameraManager Instance;
    public Camera playerCamera;
    DogPatrolBehaviour currentDog;
    //public LayerMask monsterLayer;
    public CanvasGroup flash;
    public CanvasGroup phoneFrame;
    public Light cameraFlashlight;
    private float normalIntensity;
    public GameObject gameplayDot;
    public GameObject whiteFocus;
    public GameObject redFocus;
    private Monster currentTarget;
    public float photoRange = 15f;

    [Header("UI")]
    public RectTransform cameraUI;

    [Header("Animation")]
    public Vector2 hiddenPosition;
    public Vector2 visiblePosition;
    public float slideDuration = 0.35f;

    public float hiddenRotation = -15f;
    public float visibleRotation = 0f;

    public KeyCode cameraKey = KeyCode.C;

    [Header("Battery")]
    public float battery = 100f;
    public float drainPerSecond = 0.2f;
    public float photoCost = 2f;

    public float BatteryPercent => battery;

    [Header("Battery UI")]
    public Image batteryIcon;
    public TMP_Text batteryText;

    public Sprite batteryFull;
    public Sprite battery75;
    public Sprite battery50;
    public Sprite battery25;
    public Sprite batteryEmpty;

    public bool CameraOpen { get; private set; }

    private bool isAnimating;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cameraUI.anchoredPosition = hiddenPosition;
        cameraUI.localEulerAngles = new Vector3(0, 0, hiddenRotation);
        cameraFlashlight.enabled = false;
        normalIntensity = cameraFlashlight.intensity;

        UpdateBatteryUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(cameraKey) && !isAnimating)
        {
            ToggleCamera();
        }

        if (CameraOpen)
        {
            DetectDogTarget();
            UpdateFocus();
        }
        if (CameraOpen && Input.GetMouseButtonDown(0))
        {
            TakePhoto();
        }

        if (CameraOpen && battery > 0f)
        {
            battery -= drainPerSecond * Time.deltaTime;

            battery = Mathf.Clamp(battery, 0f, 100f);

            UpdateBatteryUI();

            if (battery <= 0f)
            {
                ToggleCamera();
            }
        }

        if (!CameraOpen)
        {
            if (battery <= 0f)
                return;
        }
    }

    public void ToggleCamera()
    {
        isAnimating = true;

        CameraOpen = !CameraOpen;

        cameraFlashlight.enabled = CameraOpen;

        cameraUI.DOKill();
        phoneFrame.DOKill();

        if (!CameraOpen)
        {
            SoundManager.Instance.PlaySFX("PhoneHide");
            // Before lowering, make the frame visible again
            phoneFrame.alpha = 1f;
            gameplayDot.SetActive(true);

            whiteFocus.SetActive(false);
            redFocus.SetActive(false);

        }
        else
        {
            SoundManager.Instance.PlaySFX("PhoneGrab");
            gameplayDot.SetActive(false);

            whiteFocus.SetActive(true);
            redFocus.SetActive(false);
        }

            Sequence seq = DOTween.Sequence();

        seq.Join(cameraUI.DOAnchorPos(
            CameraOpen ? visiblePosition : hiddenPosition,
            slideDuration));

        seq.Join(cameraUI.DOLocalRotate(
            new Vector3(0, 0, CameraOpen ? visibleRotation : hiddenRotation),
            slideDuration));

        if (CameraOpen)
        {
            // Fade out while raising
            seq.Join(phoneFrame.DOFade(0f, slideDuration * 0.8f));
        }
        else
        {
            // Fade in while lowering
            phoneFrame.alpha = 0f;
            seq.Join(phoneFrame.DOFade(1f, slideDuration * 0.8f));
        }

        seq.SetEase(Ease.OutCubic);

        seq.OnComplete(() =>
        {
            isAnimating = false;
        });
    }
    void UpdateFocus()
    {
        currentTarget = null;

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit, photoRange, photoLayer))
        {
            
            if (hit.collider.TryGetComponent<PhotoTarget>(out var target))
            {
                whiteFocus.SetActive(false);
                redFocus.SetActive(true);

                if (target.monster != null)
                    currentTarget = target.monster;

                return;
            }
        }

        whiteFocus.SetActive(true);
        redFocus.SetActive(false);
    }

    void DetectDogTarget()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit, photoRange, photoLayer))
        {
            DogPatrolBehaviour dog =
                hit.collider.GetComponentInParent<DogPatrolBehaviour>();

            if (dog != null)
            {
                if (dog != currentDog)
                {
                    currentDog = dog;
                    dog.Alert();
                }
            }
            else
            {
                currentDog = null;
            }
        }
        else
        {
            currentDog = null;
        }
    }
    void Flash()
    {
        Debug.Log("FLASH!");
        // UI flash
        flash.alpha = 1f;
        flash.DOKill();
        flash.DOFade(0f, 0.15f);

        // Real flashlight flash
        cameraFlashlight.intensity = normalIntensity * 4f;

        DOVirtual.DelayedCall(0.08f, () =>
        {
            cameraFlashlight.intensity = normalIntensity;
        });
    }

    void TakePhoto()
    {
        if (battery <= 0f)
            return;

        battery -= photoCost;
        battery = Mathf.Clamp(battery, 0f, 100f);
        UpdateBatteryUI();

        if (battery <= 0f)
        {
            ToggleCamera();
            return;
        }

        SoundManager.Instance.PlaySFX("CameraShutter");
        Flash();

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit, photoRange, photoLayer))
        {
            Debug.Log("Hit: " + hit.collider.name);

            DogPatrolBehaviour patrol =
                hit.collider.GetComponentInParent<DogPatrolBehaviour>();

            if (patrol != null)
            {
                Debug.Log("Found patrol!");
                patrol.Alert();
            }
            else
            {
                Debug.Log("No patrol found.");
            }

            // Existing photo logic
            PhotoTarget target = hit.collider.GetComponent<PhotoTarget>();

            if (target != null)
                target.Photograph();
        }
    }

    void UpdateBatteryUI()
    {
        if (batteryText != null)
            batteryText.text = Mathf.RoundToInt(battery) + "%";
        if (batteryIcon == null)
            return;

        if (battery > 75f)
            batteryIcon.sprite = batteryFull;
        else if (battery > 50f)
            batteryIcon.sprite = battery75;
        else if (battery > 25f)
            batteryIcon.sprite = battery50;
        else if (battery > 0f)
            batteryIcon.sprite = battery25;
        else
            batteryIcon.sprite = batteryEmpty;
    }
    public void RechargeBattery(float amount)
    {
        battery += amount;
        battery = Mathf.Clamp(battery, 0f, 100f);

        UpdateBatteryUI();
    }
}