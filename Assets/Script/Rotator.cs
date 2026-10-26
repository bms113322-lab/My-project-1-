using UnityEngine;

/// <summary>
/// Cube에 붙여서 사용하는 회전 스크립트.
/// - 키 조작 끄기(기본): Y축으로 자동 회전 (초당 90도)
/// - 키 조작 켜기: X 키를 누르는 동안 X축 회전, Y 키를 누르는 동안 Y축 회전
/// </summary>
public class Rotator : MonoBehaviour
{
    [Header("회전 속도 (초당 각도)")]
    public float rotationSpeed = 90f;

    [Header("모드")]
    [Tooltip("끄면 Y축 자동 회전, 켜면 X/Y 키를 눌러서 회전")]
    public bool useKeyControl = false;

    void Update()
    {
        // Time.deltaTime을 곱해서 프레임 속도와 상관없이 초당 일정한 각도로 회전
        float step = rotationSpeed * Time.deltaTime;

        if (!useKeyControl)
        {
            // 자동 모드: 항상 Y축으로 회전
            transform.Rotate(0f, step, 0f);
        }
        else
        {
            // 키 모드: 누르고 있는 동안만 해당 축으로 회전
            if (Input.GetKey(KeyCode.X))
            {
                transform.Rotate(step, 0f, 0f);
            }

            if (Input.GetKey(KeyCode.Y))
            {
                transform.Rotate(0f, step, 0f);
            }
        }
    }
}