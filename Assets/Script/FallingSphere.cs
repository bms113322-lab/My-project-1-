using UnityEngine;

/// <summary>
/// 떨어지는 Sphere에 붙어서 플레이어와 닿으면 Game Over를 호출.
/// CharacterController와도 안정적으로 감지되도록
/// 일반 Collider(바닥 충돌용) + 약간 큰 Trigger Collider를 같이 사용.
/// </summary>
public class FallingSphere : MonoBehaviour
{
    void Awake()
    {
        SphereCollider solid = GetComponent<SphereCollider>();
        if (solid != null)
        {
            SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.center = solid.center;
            trigger.radius = solid.radius * 1.1f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        CheckPlayer(collision.collider);
    }

    void OnTriggerEnter(Collider other)
    {
        CheckPlayer(other);
    }

    void CheckPlayer(Collider other)
    {
        // PlayerController가 붙은 오브젝트(또는 그 자식)에 닿았는지 확인
        if (other.GetComponentInParent<PlayerController>() == null) return;

        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
    }
}