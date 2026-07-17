using DG.Tweening;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public LayerMask photoLayer;
    public static CameraManager Instance;
    public Camera playerCamera;
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
    }

    void Update()
    {
        if (Input.GetKeyDown(cameraKey) && !isAnimating)
        {
            ToggleCamera();
        }

        if (CameraOpen)
        {
            UpdateFocus();
        }
        if (CameraOpen && Input.GetMouseButtonDown(0))
        {
            TakePhoto();
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
            // Before lowering, make the frame visible again
            phoneFrame.alpha = 1f;
            gameplayDot.SetActive(true);

            whiteFocus.SetActive(false);
            redFocus.SetActive(false);

        }
        else
        {
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
        bool targetDetected = false;
        currentTarget = null;
        foreach (Monster monster in Monster.ActiveMonsters)
        {
        if (monster.photoTarget == null)
                continue;

            Vector3 viewportPos =
                playerCamera.WorldToViewportPoint(monster.photoTarget.position);

            if (viewportPos.z <= 0)
                continue;

            float distance = Vector3.Distance(
                playerCamera.transform.position,
                monster.photoTarget.position);

            if (distance > photoRange)
                continue;

            float offset = Vector2.Distance(
                new Vector2(viewportPos.x, viewportPos.y),
                new Vector2(0.5f, 0.5f));

            float photoDistance = 2.25f;

            if (offset < 0.15f && distance <= photoDistance)
            {
                targetDetected = true;
                currentTarget = monster;
            }
        }

        whiteFocus.SetActive(!targetDetected);
        redFocus.SetActive(targetDetected);
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
        Flash();

        Ray ray = playerCamera.ViewportPointToRay(
                new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit, photoRange, photoLayer))
        {
            PhotoTarget target = hit.collider.GetComponent<PhotoTarget>();

            if (target != null)
                target.Photograph();
        }
        //if (currentTarget != null)
        //{
        //    currentTarget.Disperse();
        //}
    }
}