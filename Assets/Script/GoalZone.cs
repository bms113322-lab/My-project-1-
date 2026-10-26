using UnityEngine;

/// <summary>
/// 플레이어가 닿으면 클리어 처리를 하는 골인 지점.
/// Collider가 있는 오브젝트에 붙이면 자동으로 Trigger로 설정됨.
/// </summary>
public class GoalZone : MonoBehaviour
{
    void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning($"[GoalZone] {name}에 Collider가 없습니다.");
            return;
        }
        col.isTrigger = true;   // 통과하면서 감지되도록
    }

    void OnTriggerEnter(Collider other)
    {
        // PlayerController가 붙은 오브젝트(또는 그 자식)에 닿았는지 확인
        if (other.GetComponentInParent<PlayerController>() == null) return;

        if (GameManager.Instance != null)
            GameManager.Instance.Clear();
    }
}