using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Camera Movement")]
    public float cameraMoveMultiplier = 0.6f;
    public bool CanMove { get; private set; } = true;

    [Header("Movement")]
    public float walkSpeed = 1.5f;
    public float sprintSpeed = 3f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public Transform cameraPivot;
    public float mouseSensitivity = 0.01f;
    public float minLookAngle = -70f;
    public float maxLookAngle = 70f;

    [Header("Footsteps")]
    public float walkStepInterval = 0.5f;    // Time between steps while walking
    public float sprintStepInterval = 0.3f;  // Faster steps while sprinting
    public float raycastDistance = 1.5f;     // Ray distance down to detect surface tag
    public LayerMask groundLayer = ~0;       // Layer mask for ground colliders

    private Animator animator;
    private CharacterController controller;
    private Controls controls;

    private float yaw;
    private float pitch;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintHeld;

    private Vector3 velocity;
    private float stepTimer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        controls = new Controls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        controls.Player.Sprint.performed += ctx => sprintHeld = true;
        controls.Player.Sprint.canceled += ctx => sprintHeld = false;

        yaw = transform.eulerAngles.y;

        pitch = cameraPivot.localEulerAngles.x;
        if (pitch > 180f)
            pitch -= 360f;
    }

    private void OnEnable()
    {
        controls.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        Look();
        Move();
    }

    public void SetMovement(bool enabled)
    {
        CanMove = enabled;

        if (!enabled)
        {
            moveInput = Vector2.zero;

            if (animator != null)
                animator.SetFloat("Speed", 0);
        }
    }

    private void Look()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minLookAngle, maxLookAngle);

        // Smooth rotation
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(0f, yaw, 0f),
            10f * Time.deltaTime);

        cameraPivot.localRotation = Quaternion.Slerp(
            cameraPivot.localRotation,
            Quaternion.Euler(pitch, 0f, 0f),
            10f * Time.deltaTime);
    }

    private void Move()
    {
        if (!CanMove)
        {
            if (controller.isGrounded && velocity.y < 0)
                velocity.y = -2f;

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            stepTimer = 0f; // Reset step timer if movement is disabled
            return;
        }

        Vector3 move =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        bool cameraOpen =
            CameraManager.Instance != null &&
            CameraManager.Instance.CameraOpen;

        // Sprint is disabled while the camera is up
        bool isSprinting = !cameraOpen && sprintHeld;
        float speed = isSprinting ? sprintSpeed : walkSpeed;

        // Slow movement while using the camera
        if (cameraOpen)
        {
            speed *= cameraMoveMultiplier;
        }

        controller.Move(speed * Time.deltaTime * move);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Handle footsteps timing and audio triggering
        HandleFootsteps(moveInput.sqrMagnitude > 0.01f, isSprinting, cameraOpen);

        if (animator != null)
        {
            float animationSpeed = moveInput.magnitude;

            if (!cameraOpen && sprintHeld && animationSpeed > 0)
                animationSpeed = 2f;
            else if (animationSpeed > 0)
                animationSpeed = 1f;

            animator.SetFloat("Speed", animationSpeed);
        }

        if (moveInput.sqrMagnitude > 0.01f &&
            ObjectiveManager.Instance != null &&
            ObjectiveManager.Instance.IsCurrentObjective("Move"))
        {
            ObjectiveManager.Instance.CompleteObjective();
        }
    }

    private void HandleFootsteps(bool isMoving, bool isSprinting, bool cameraOpen)
    {
        // Only trigger steps if grounded and actively inputting movement
        if (!controller.isGrounded || !isMoving)
        {
            stepTimer = 0f;
            return;
        }

        // Determine current interval between step sounds
        float currentInterval = isSprinting ? sprintStepInterval : walkStepInterval;
        if (cameraOpen) currentInterval *= 1.3f; // Slightly slower cadence when camera is raised

        stepTimer += Time.deltaTime;

        if (stepTimer >= currentInterval)
        {
            PlayFootstepSound();
            stepTimer = 0f;
        }
    }

    private void PlayFootstepSound()
    {
        // Cast a short ray down from the player controller origin
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer))
        {
            string surfaceTag = hit.collider.tag;
            string soundToPlay = "";

            // Map ground tag to SoundManager SFX library key
            switch (surfaceTag)
            {
                case "Concrete":
                    soundToPlay = "Footstep_Concrete";
                    break;

                case "Dirt":
                    soundToPlay = "Footstep_Dirt";
                    break;

                default:
                    soundToPlay = "Footstep_Default";
                    break;
            }

            if (SoundManager.Instance != null && !string.IsNullOrEmpty(soundToPlay))
            {
                SoundManager.Instance.PlaySFX(soundToPlay);
            }
        }
    }
}