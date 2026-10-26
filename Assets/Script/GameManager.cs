using UnityEngine;
using TMPro;

/// <summary>
/// 생존 시간 측정, UI 표시, Game Over / Clear 처리를 담당.
/// 씬에 하나만 두면 됨.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI 연결")]
    public TMP_Text timerText;          // 좌상단 시간 텍스트
    public GameObject gameOverPanel;    // Game Over 때 켤 UI (평소엔 꺼짐)
    public TMP_Text resultText;         // Game Over 패널 안의 결과 텍스트
    public GameObject clearPanel;       // Clear 때 켤 UI (평소엔 꺼짐)
    public TMP_Text clearText;          // Clear 패널 안의 텍스트

    public bool IsGameOver { get; private set; }
    public bool IsCleared { get; private set; }
    public bool IsFinished => IsGameOver || IsCleared;   // 어느 쪽이든 끝났는지
    public float SurvivalTime { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        IsGameOver = false;
        IsCleared = false;
        SurvivalTime = 0f;

        // 시작할 때 결과 UI는 숨김
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (clearPanel != null) clearPanel.SetActive(false);
    }

    void Update()
    {
        // 끝난 뒤에는 시간이 더 이상 늘지 않음
        if (IsFinished) return;

        SurvivalTime += Time.deltaTime;

        if (timerText != null)
            timerText.text = $"Time: {SurvivalTime:F1}s";
    }

    public void GameOver()
    {
        if (IsFinished) return;   // 이미 끝났으면 무시
        IsGameOver = true;

        Debug.Log("Game Over");

        if (timerText != null)
            timerText.text = $"Time: {SurvivalTime:F1}s";

        if (resultText != null)
            resultText.text = $"Game Over\nSurvived: {SurvivalTime:F1}s";

        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        ShowCursor();
    }

    public void Clear()
    {
        if (IsFinished) return;   // Game Over 후 또는 중복 클리어 방지
        IsCleared = true;

        Debug.Log($"Clear {SurvivalTime:F1}s");

        if (timerText != null)
            timerText.text = $"Time: {SurvivalTime:F1}s";

        if (clearText != null)
            clearText.text = $"Clear!\nTime: {SurvivalTime:F1}s";

        if (clearPanel != null) clearPanel.SetActive(true);

        ShowCursor();
    }

    // 1인칭 모드에서 잠가둔 마우스 커서를 풀어줌
    void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}