
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FoxController : MonoBehaviour
{
    [Header("이동 설정")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float gravity = -20f;

    [Header("여우 모델")]
    public Transform foxModel;

    [Header("카메라 설정")]
    public Transform cameraTransform;

    [Tooltip("좌우 마우스 감도")]
    [Range(0.01f, 2f)]
    public float horizontalSensitivity = 0.15f;

    [Tooltip("상하 마우스 감도")]
    [Range(0.01f, 2f)]
    public float verticalSensitivity = 0.15f;

    [Tooltip("마우스 상하 반전")]
    public bool invertY = false;

    public float minCameraAngle = -30f;
    public float maxCameraAngle = 70f;

    [Header("애니메이션 설정")]
    public Animator animator;
    public string speedParameter = "Speed";


    private CharacterController controller;

    private float cameraYaw;
    private float cameraPitch;
    private float verticalVelocity;

    private Vector3 cameraLocalOffset;
    private float animationSpeed;

    void Start()
    {


        controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // 현재 카메라 위치와 각도를 그대로 저장
        if (cameraTransform != null)
        {
            cameraYaw = cameraTransform.eulerAngles.y;

            cameraPitch = NormalizeAngle(
                cameraTransform.eulerAngles.x
            );

            Vector3 worldOffset =
                cameraTransform.position - transform.position;

            cameraLocalOffset =
                Quaternion.Inverse(
                    Quaternion.Euler(0f, cameraYaw, 0f)
                ) * worldOffset;
        }
        else
        {
            cameraYaw = transform.eulerAngles.y;
            cameraPitch = 0f;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (animator != null)
            animator.applyRootMotion = false;
    }

    void Update()
    {
        if (GameManager.instance.isGameOver) return;
        HandleCursor();
        HandleMouseLook();
        HandleMovement();
        HandleAnimation();
    }

    void LateUpdate()
    {
        if (GameManager.instance.isGameOver) return;
        UpdateCamera();
    }

    void HandleCursor()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void HandleMouseLook()
    {
        if (Mouse.current == null ||
            Cursor.lockState != CursorLockMode.Locked)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // 좌우 감도 적용
        float yawChange =
            mouseDelta.x * horizontalSensitivity;

        // 상하 감도 적용
        float pitchChange =
            mouseDelta.y * verticalSensitivity;

        if (invertY)
            pitchChange = -pitchChange;

        cameraYaw += yawChange;
        cameraPitch -= pitchChange;

        cameraPitch = Mathf.Clamp(
            cameraPitch,
            minCameraAngle,
            maxCameraAngle
        );

        // 여우도 카메라와 함께 좌우 회전
        transform.Rotate(
            0f,
            yawChange,
            0f,
            Space.World
        );
    }

    Vector2 GetMovementInput()
    {
        if (Keyboard.current == null)
            return Vector2.zero;

        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            input.y += 1f;

        if (Keyboard.current.sKey.isPressed)
            input.y -= 1f;

        if (Keyboard.current.aKey.isPressed)
            input.x -= 1f;

        if (Keyboard.current.dKey.isPressed)
            input.x += 1f;

        return Vector2.ClampMagnitude(input, 1f);
    }

    bool IsRunning()
    {
        if (Keyboard.current == null)
            return false;

        return Keyboard.current.leftShiftKey.isPressed ||
               Keyboard.current.rightShiftKey.isPressed;
    }

    void HandleMovement()
    {
        Vector2 input = GetMovementInput();

        Vector3 cameraForward = transform.forward;
        Vector3 cameraRight = transform.right;

        if (cameraTransform != null)
        {
            cameraForward = cameraTransform.forward;
            cameraRight = cameraTransform.right;
        }

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection =
            cameraRight * input.x +
            cameraForward * input.y;

        moveDirection = Vector3.ClampMagnitude(
            moveDirection, 1f
        );

        bool isMoving = input.sqrMagnitude > 0.01f;
        bool running = IsRunning() && isMoving;

        float speed = running ? runSpeed : walkSpeed;

        Vector3 movement = moveDirection * speed;

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        movement.y = verticalVelocity;

        controller.Move(movement * Time.deltaTime);
    }

    void HandleAnimation()
    {
        if (animator == null)
            return;

        Vector2 input = GetMovementInput();

        bool isMoving = input.sqrMagnitude > 0.01f;
        bool running = IsRunning() && isMoving;

        float targetSpeed = 0f;

        if (isMoving)
            targetSpeed = running ? 2f : 1f;

        animationSpeed = Mathf.MoveTowards(
            animationSpeed,
            targetSpeed,
            Time.deltaTime * 10f
        );

        animator.SetFloat(
            speedParameter,
            animationSpeed
        );
    }

    void UpdateCamera()
    {
        if (cameraTransform == null)
            return;

        Quaternion yawRotation = Quaternion.Euler(
            0f,
            cameraYaw,
            0f
        );

        // 시작할 때 맞춰놓은 카메라 거리 유지
        Vector3 desiredPosition =
            transform.position +
            yawRotation * cameraLocalOffset;

        cameraTransform.position = desiredPosition;

        cameraTransform.rotation = Quaternion.Euler(
            cameraPitch,
            cameraYaw,
            0f
        );
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }
}
