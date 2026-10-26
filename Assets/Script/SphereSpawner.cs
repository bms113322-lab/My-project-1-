using UnityEngine;

/// <summary>
/// 일정 간격으로 "지정된 오브젝트 주변"의 공중에서 Sphere를 생성.
/// - spawnPoints에 오브젝트를 넣으면: 그 오브젝트들 위에서 순서대로 돌아가며 생성
/// - spawnPoints가 비어 있으면: target(예: 플레이어) 주변에서 생성
/// - 둘 다 비어 있으면: 이 스포너 오브젝트 위치 기준으로 생성
/// </summary>
public class SphereSpawner : MonoBehaviour
{
    [Header("생성 설정")]
    public GameObject spherePrefab;         // 비워두면 기본 Sphere를 코드로 생성
    public float spawnInterval = 3f;        // 생성 간격(초)
    public float lifeTime = 6f;             // 생성 후 자동 삭제까지 시간(초)
    public float sphereSize = 1f;           // Sphere 크기 (프리팹 미사용 시)

    [Header("생성 기준 오브젝트")]
    public Transform target;                // 이 오브젝트 주변에서 생성 (예: Player)
    public Transform[] spawnPoints;         // 지정하면 target 대신 이 오브젝트들 위에서 순서대로 생성

    [Header("생성 위치")]
    public float radius = 2f;               // 기준 오브젝트 주변 반경 (0이면 정확히 머리 위)
    public float spawnHeight = 10f;         // 기준 오브젝트로부터의 높이

    private float timer;
    private int pointIndex;                 // spawnPoints 순서 기록

    void Update()
    {
        // Game Over 후에는 더 이상 생성하지 않음
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer -= spawnInterval;
            Spawn();
        }
    }

    // 이번에 사용할 기준 위치를 결정
    Transform GetBase()
    {
        // 지정된 오브젝트 목록이 있으면 순서대로 사용
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform t = spawnPoints[pointIndex % spawnPoints.Length];
            pointIndex++;
            return t;
        }

        // 없으면 target, 그것도 없으면 스포너 자신
        return target != null ? target : transform;
    }

    void Spawn()
    {
        Transform baseObj = GetBase();
        if (baseObj == null) return;   // 지정한 오브젝트가 삭제된 경우 대비

        // 기준 오브젝트 주변 원 안의 위치 (radius가 0이면 offset도 0)
        Vector2 offset = Random.insideUnitCircle * radius;
        Vector3 pos = baseObj.position + new Vector3(offset.x, spawnHeight, offset.y);

        GameObject sphere;
        if (spherePrefab != null)
        {
            sphere = Instantiate(spherePrefab, pos, Quaternion.identity);
        }
        else
        {
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = pos;
            sphere.transform.localScale = Vector3.one * sphereSize;
        }
        sphere.name = "FallingSphere";

        // Rigidbody로 중력 낙하
        Rigidbody rb = sphere.GetComponent<Rigidbody>();
        if (rb == null) rb = sphere.AddComponent<Rigidbody>();
        rb.useGravity = true;

        // 플레이어 충돌 감지 스크립트
        if (sphere.GetComponent<FallingSphere>() == null)
            sphere.AddComponent<FallingSphere>();

        // lifeTime초 뒤 자동 삭제
        Destroy(sphere, lifeTime);
    }

    // 씬 뷰에서 스포너를 선택하면 생성 위치를 노란 원으로 표시
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            foreach (Transform t in spawnPoints)
                if (t != null) DrawArea(t.position);
        }
        else
        {
            Transform b = target != null ? target : transform;
            DrawArea(b.position);
        }
    }

    void DrawArea(Vector3 center)
    {
        Vector3 top = center + Vector3.up * spawnHeight;
        Gizmos.DrawWireSphere(top, Mathf.Max(radius, 0.1f));
        Gizmos.DrawLine(center, top);   // 기준 오브젝트와 생성 높이를 잇는 선
    }
}