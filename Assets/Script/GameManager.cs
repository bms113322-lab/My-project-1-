using UnityEngine;
using TMPro;

/// <summary>
/// 생존 시간 측정, UI 표시, Game Over / Clear 처리를 담당.
/// 타이머는 게임 중, Game Over, Clear 모두 화면에 계속 표시됨.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI 연결")]
    public TMP_Text timerText;          // 좌상단 시간 텍스트 (항상 표시)
    public GameObject gameOverPanel;    // Game Over 때 켤 UI (평소엔 꺼짐)
    public TMP_Text resultText;         // Game Over 패널 안의 결과 텍스트
    public GameObject clearPanel;       // Clear 때 켤 UI (평소엔 꺼짐)
    public TMP_Text clearText;          // Clear 패널 안의 텍스트

    public bool IsGameOver { get; private set; }
    public bool IsCleared { get; private set; }
    public bool IsFinished => IsGameOver || IsCleared;
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

        // 시작할 때 결과 패널은 숨기고, 타이머는 반드시 켬
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (clearPanel != null) clearPanel.SetActive(false);

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            UpdateTimerText();
        }
    }

    void Update()
    {
        // 끝난 뒤에는 시간이 늘지 않지만, 텍스트는 그대로 화면에 남음
        if (IsFinished) return;

        SurvivalTime += Time.deltaTime;
        UpdateTimerText();
    }

    void UpdateTimerText()
    {
        if (timerText != null)
            timerText.text = $"Time: {SurvivalTime:F1}s";
    }

    public void GameOver()
    {
        if (IsFinished) return;
        IsGameOver = true;

        Debug.Log("Game Over");

        UpdateTimerText();   // 최종 시간으로 고정 (타이머는 계속 표시됨)

        if (resultText != null)
            resultText.text = $"Game Over\nSurvived: {SurvivalTime:F1}s";

        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        ShowCursor();
    }

    public void Clear()
    {
        if (IsFinished) return;
        IsCleared = true;

        Debug.Log($"Clear {SurvivalTime:F1}s");

        UpdateTimerText();   // 최종 시간으로 고정 (타이머는 계속 표시됨)

        if (clearText != null)
            clearText.text = $"Clear!\nTime: {SurvivalTime:F1}s";

        if (clearPanel != null) clearPanel.SetActive(true);

        ShowCursor();
    }

    void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}