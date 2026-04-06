using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(GravityController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float acceleration = 20f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float jumpBufferTime = 0.15f;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Gravity")]
    [SerializeField] private float maxFallSpeed = 40f;
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    private Rigidbody rb;
    private PlayerInput playerInput;
    private GravityController gravityController;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 moveInput;
    private Vector3 currentHorizontalVelocity;
    private float jumpBufferCounter;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        gravityController = GetComponent<GravityController>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
    }

    private void OnEnable()
    {
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];

        if (moveAction != null) { moveAction.performed += OnMove; moveAction.canceled += OnMoveCanceled; }
        else Debug.LogError("Move action not found.", this);

        if (jumpAction != null) jumpAction.performed += OnJump;
        else Debug.LogError("Jump action not found.", this);

        gravityController.OnGravityChanged += OnGravityChanged;
    }

    private void OnDisable()
    {
        if (moveAction != null) { moveAction.performed -= OnMove; moveAction.canceled -= OnMoveCanceled; }
        if (jumpAction != null) jumpAction.performed -= OnJump;

        gravityController.OnGravityChanged -= OnGravityChanged;
    }

    private void OnMove(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext ctx) => moveInput = Vector2.zero;
    private void OnJump(InputAction.CallbackContext ctx) => jumpBufferCounter = jumpBufferTime;
    private void OnGravityChanged(Vector3 _) => currentHorizontalVelocity = Vector3.zero;

    private void FixedUpdate()
    {
        Vector3 gravityUp = gravityController.GetGravityUp();

        jumpBufferCounter -= Time.fixedDeltaTime;

        GroundCheck(gravityUp);
        ApplyGravity(gravityUp);
        Movement(gravityUp);
        Rotation(gravityUp);
        Jump(gravityUp);
        UpdateAnimation(gravityUp);
    }

    private void GroundCheck(Vector3 gravityUp)
    {
        Vector3 origin = transform.position + gravityUp * 0.1f;
        isGrounded = Physics.SphereCast(origin, groundCheckRadius, -gravityUp, out RaycastHit _, groundCheckDistance + 0.1f, groundLayer);
    }

    private void ApplyGravity(Vector3 gravityUp)
    {
        Vector3 vel = rb.linearVelocity;
        float verticalSpeed = Vector3.Dot(vel, gravityUp);
        if (isGrounded)
        {
            if (verticalSpeed < 0f)
            {
                vel -= gravityUp * verticalSpeed;
                rb.linearVelocity = vel;
            }
            return;
        }
        
        vel += gravityController.GetGravityDirection() * gravityController.GetGravityStrength() * Time.fixedDeltaTime;
        
        float newVerticalSpeed = Vector3.Dot(vel, gravityUp);
        
        if (newVerticalSpeed < -maxFallSpeed)
            vel -= gravityUp * (newVerticalSpeed + maxFallSpeed);
        
        rb.linearVelocity = vel;
    }

    private void Movement(Vector3 gravityUp)
    {
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, gravityUp).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, gravityUp).normalized;
        Vector3 targetHorizontal = inputDir.magnitude > 0.1f
            ? (camForward * inputDir.z + camRight * inputDir.x) * moveSpeed
            : Vector3.zero;
        currentHorizontalVelocity = Vector3.Lerp(currentHorizontalVelocity, targetHorizontal,
            1f - Mathf.Exp(-acceleration * Time.fixedDeltaTime));
        Vector3 verticalVel = Vector3.Project(rb.linearVelocity, gravityUp);
        rb.linearVelocity = currentHorizontalVelocity + verticalVel;
    }

    private void Rotation(Vector3 gravityUp)
    {
        if (gravityController.IsRotating()) return;
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
        if (inputDir.sqrMagnitude < 0.01f) return;
        Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, gravityUp).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, gravityUp).normalized;
        Vector3 moveDir = camForward * inputDir.z + camRight * inputDir.x;
        Quaternion targetRotation = Quaternion.LookRotation(moveDir, gravityUp);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
    }

    private void Jump(Vector3 gravityUp)
    {
        if (jumpBufferCounter <= 0f || !isGrounded) return;
        jumpBufferCounter = 0f;
        Vector3 vel = rb.linearVelocity;
        vel -= gravityUp * Vector3.Dot(vel, gravityUp);
        vel += gravityUp * jumpForce;
        rb.linearVelocity = vel;
    }

    private void UpdateAnimation(Vector3 gravityUp)
    {
        float horizontalSpeed = Vector3.ProjectOnPlane(rb.linearVelocity, gravityUp).magnitude;
        float normalizedSpeed = Mathf.Clamp01(horizontalSpeed / moveSpeed);
        if (animator != null)
        {
            animator.SetFloat(SpeedHash, normalizedSpeed, 0.1f, Time.fixedDeltaTime);
            animator.SetBool(IsGroundedHash, isGrounded);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (gravityController == null) return;
        Vector3 gravityUp = gravityController.GetGravityUp();
        Vector3 origin = transform.position + gravityUp * 0.1f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, groundCheckRadius);
        Gizmos.DrawLine(origin, origin + (-gravityUp) * (groundCheckDistance + 0.1f));
    }
}