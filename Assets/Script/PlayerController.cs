using UnityEngine;
using UnityEngine.InputSystem;   // 새 Input System

/// <summary>
/// 1인칭 플레이어 컨트롤러 (새 Input System 버전).
/// - 마우스: 좌우는 몸통 회전, 상하는 카메라 회전
/// - WASD: 바라보는 방향 기준 이동
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("이동")]
    public float moveSpeed = 5f;            // 이동 속도 (초당 유닛)
    public float gravity = -20f;            // 아래로 당기는 힘

    [Header("시점")]
    public Transform cameraTransform;       // Main Camera (비워두면 자동으로 찾음)
    public float mouseSensitivity = 0.1f;   // 마우스 감도 (새 시스템은 값 단위가 커서 기본값이 작음)
    public float maxLookAngle = 85f;        // 위아래 시야 제한 각도

    private CharacterController controller;
    private float verticalVelocity;
    private float pitch;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        Look();
        Move();
    }

    void Look()
    {
        if (Mouse.current == null) return;

        // 이번 프레임의 마우스 이동량
        Vector2 delta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        // 좌우: 몸 전체를 Y축으로 회전
        transform.Rotate(0f, delta.x, 0f);

        // 상하: 카메라만 X축으로 회전 (위아래 제한)
        pitch -= delta.y;
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Move()
    {
        if (Keyboard.current == null) return;

        // WASD 입력을 -1 ~ 1 값으로 변환
        float h = 0f;
        float v = 0f;
        if (Keyboard.current.dKey.isPressed) h += 1f;
        if (Keyboard.current.aKey.isPressed) h -= 1f;
        if (Keyboard.current.wKey.isPressed) v += 1f;
        if (Keyboard.current.sKey.isPressed) v -= 1f;

        // 바라보는 방향 기준 이동, 대각선 속도 보정
        Vector3 move = transform.right * h + transform.forward * v;
        move = Vector3.ClampMagnitude(move, 1f);

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * moveSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
}