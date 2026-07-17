using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    [Header("Movement")]
    public float walkSpeed = 2.8f;
    public float sprintSpeed = 4.8f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public Transform cameraPivot;
    public float mouseSensitivity = 0.01f;
    public float minLookAngle = -70f;
    public float maxLookAngle = 70f;
    
    private Animator animator;
    private CharacterController controller;
    private Controls controls;

    private float yaw;
    private float pitch;


    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintHeld;

    private Vector3 velocity;
    private float cameraPitch;

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

            return;
        }

        Vector3 move =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        float speed = sprintHeld ? sprintSpeed : walkSpeed;

        controller.Move(move * speed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        if (animator != null)
        {
            float animationSpeed = moveInput.magnitude;

            if (sprintHeld && animationSpeed > 0)
                animationSpeed = 2f;   // Running
            else if (animationSpeed > 0)
                animationSpeed = 1f;   // Walking

            animator.SetFloat("Speed", animationSpeed);
        }

        if (moveInput.sqrMagnitude > 0.01f &&
            ObjectiveManager.Instance.IsCurrentObjective("Move"))
        {
            ObjectiveManager.Instance.CompleteObjective();
        }
    }
}