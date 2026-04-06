using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform pivot; // PlayerCameraPivot transform

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float verticalMinClamp = -30f;
    [SerializeField] private float verticalMaxClamp = 60f;
    [SerializeField] private bool invertY = false;

    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.05f;

    private PlayerInput playerInput;
    private InputAction lookAction;

    private float yaw;    // horizontal rotation
    private float pitch;  // vertical rotation

    private float currentYaw;
    private float currentPitch;
    private float yawVelocity;
    private float pitchVelocity;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initial Yaw should be player's default
        yaw = pivot.eulerAngles.y;
        currentYaw = yaw;
    }

    private void OnEnable()
    {
        lookAction = playerInput.actions["Look"];

        if (lookAction == null)
            Debug.LogError("Look action not found. Add a 'Look' action to your Input Action Asset.", this);
    }

    private void OnDisable()
    {
        lookAction = null;
    }

    private void Update()
    {
        HandleMouseLook();
    }

    private void HandleMouseLook()
    {
        if (lookAction == null) return;

        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity * (invertY ? 1f : -1f);

        yaw   += mouseX;
        pitch += mouseY;

        // Clamp vertical view
        pitch = Mathf.Clamp(pitch, verticalMinClamp, verticalMaxClamp);

        // roation smoothness i guess
        currentYaw   = Mathf.SmoothDampAngle(currentYaw,   yaw,   ref yawVelocity,   smoothTime);
        currentPitch = Mathf.SmoothDampAngle(currentPitch, pitch, ref pitchVelocity, smoothTime);

        pivot.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }
}