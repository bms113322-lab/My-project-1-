using UnityEngine;

/// <summary>
/// 자동 회전 + 닿아 있는 플레이어를 같이 회전시키는 스크립트.
/// Cube(BoxCollider)에 붙여서 사용.
/// </summary>
public class RotatorAuto : MonoBehaviour
{
    [Header("회전할 축 (체크하면 회전)")]
    public bool rotateX = false;
    public bool rotateY = true;    // 기본값: Y축 회전

    [Header("회전 속도 (초당 각도)")]
    public float xSpeed = 90f;
    public float ySpeed = 90f;

    [Header("플레이어 같이 회전")]
    public bool carryPlayer = true;          // 끄면 플레이어는 영향 없음
    public float triggerMargin = 0.1f;       // 감지 범위를 큐브보다 얼마나 키울지 (0.1 = 10%)

    private CharacterController player;      // 현재 닿아 있는 플레이어 (없으면 null)

    void Awake()
    {
        // CharacterController는 일반 충돌 이벤트를 못 받으므로,
        // 약간 큰 Trigger 콜라이더를 추가해서 닿았는지 감지
        BoxCollider solid = GetComponent<BoxCollider>();
        if (solid != null)
        {
            BoxCollider trigger = gameObject.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.center = solid.center;
            trigger.size = solid.size * (1f + triggerMargin);
        }
        else
        {
            Debug.LogWarning($"[RotatorAuto] {name}에 BoxCollider가 없어서 플레이어 감지가 안 됩니다.");
        }
    }

    void Update()
    {
        // 체크된 축만 속도를 적용하고, 꺼진 축은 0으로 처리
        float x = rotateX ? xSpeed * Time.deltaTime : 0f;
        float y = rotateY ? ySpeed * Time.deltaTime : 0f;

        // 회전 전 상태 저장
        Quaternion before = transform.rotation;

        transform.Rotate(x, y, 0f);

        // 닿아 있는 플레이어가 있으면 같이 움직임
        if (carryPlayer && player != null)
            CarryPlayer(before);
    }

    void CarryPlayer(Quaternion before)
    {
        // 이번 프레임에 큐브가 회전한 양
        Quaternion delta = transform.rotation * Quaternion.Inverse(before);

        // 1) 위치: 큐브 중심을 기준으로 플레이어 위치를 같이 돌림
        Vector3 offset = player.transform.position - transform.position;
        Vector3 moved = delta * offset - offset;
        player.Move(moved);

        // 2) 방향: Y축(좌우) 회전만 플레이어 몸에 적용
        float yaw = delta.eulerAngles.y;
        if (yaw > 180f) yaw -= 360f;           // 0~360을 -180~180으로 변환
        player.transform.Rotate(0f, yaw, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc != null) player = cc;
    }

    void OnTriggerExit(Collider other)
    {
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc != null && cc == player) player = null;
    }
}