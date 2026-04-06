using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPivot;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float acceleration = 20f;

    [Header("Jump Settigs")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction jumpAction;

    private Vector2 moveInput;
    private bool jumpInput;

    private Vector3 currentVelocity;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = true;

        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        // Fetch actions here so the action map is guaranteed to be active
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];

        if (moveAction != null)
        {
            moveAction.performed += OnMove;
            moveAction.canceled += OnMoveCanceled;
        }
        else
        {
            Debug.LogError("Move action not found. Check your Input Action Asset action name.", this);
        }

        if (jumpAction != null)
        {
            jumpAction.performed += OnJump;
        }
        else
        {
            Debug.LogError("Jump action not found. Check your Input Action Asset action name.", this);
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMoveCanceled;
        }

        if (jumpAction != null)
        {
            jumpAction.performed -= OnJump;
        }
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        jumpInput = true;
    }

    private void FixedUpdate()
    {
        GroundCheck();
        Movement();
        Rotation();
        Jump();
    }

    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.1f, groundLayer);
    }

    private void Movement()
    {
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        if (inputDir.magnitude < 0.1f)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            Vector3 camForward = cameraPivot.forward;
            Vector3 camRight = cameraPivot.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * inputDir.z + camRight * inputDir.x;
            currentVelocity = Vector3.Lerp(currentVelocity, moveDir * moveSpeed, acceleration * Time.fixedDeltaTime);
        }

        Vector3 velocity = rb.linearVelocity;
        velocity.x = currentVelocity.x;
        velocity.z = currentVelocity.z;
        rb.linearVelocity = velocity;
    }

    private void Rotation()
    {
        Vector3 moveDir = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            Quaternion smoothRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothRotation);
        }
    }

    private void Jump()
    {
        if (!jumpInput) return;
        jumpInput = false; // Always clear first to prevent jump getting "stuck"

        if (isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckDistance);
    }
}