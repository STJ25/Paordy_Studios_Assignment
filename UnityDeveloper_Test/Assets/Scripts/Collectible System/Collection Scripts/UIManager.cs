using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// This Manager Listens to CollectionManager events and updates the UI accordingly.
/// No game logic lives here — purely responsible for display.
/// </summary>

// Attach this component to a dedicated empty GameObject in the scene.
// Do not attach alongside other managers to keep responsibilities separate.

public class UIManager : MonoBehaviour
{
    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI collectedText;
    [SerializeField] private TextMeshProUGUI remainingText;

    [Header("Timer UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Start Panel")]
    [SerializeField] private CanvasGroup startPanelCanvasGroup;

    [Header("End Panel")]
    [SerializeField] private CanvasGroup endPanelCanvasGroup;
    [SerializeField] private TextMeshProUGUI endPanelText;

    [Header("Data")]
    [SerializeField] private CollectibleData collectibleData;

    private void OnEnable()
    {
        CollectionManager.OnItemCollected += UpdateScoreUI;
        CollectionManager.OnTimerUpdated  += UpdateTimerUI;
        CollectionManager.OnGoalReached   += HandleGoalReached;
        CollectionManager.OnTimeExpired   += HandleTimeExpired;
    }

    private void OnDisable()
    {
        CollectionManager.OnItemCollected -= UpdateScoreUI;
        CollectionManager.OnTimerUpdated  -= UpdateTimerUI;
        CollectionManager.OnGoalReached   -= HandleGoalReached;
        CollectionManager.OnTimeExpired   -= HandleTimeExpired;
    }

    private void Start()
    {
        if (collectibleData == null)
        {
            Debug.LogError("UIManager has no CollectibleData assigned.", this);
            return;
        }

        collectedText.text = $"0";
        remainingText.text = $"{collectibleData.GoalAmount}";
        timerText.text     = $"Time : {collectibleData.TimeLimit:F1}";

        SetPanel(startPanelCanvasGroup, true);
        SetPanel(endPanelCanvasGroup,   false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    public void OnStartButtonPressed()
    {
        SetPanel(startPanelCanvasGroup, false);
        CollectionManager.Instance.StartGame();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnRestartButtonPressed()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Updates Collected and remaining count Display
    private void UpdateScoreUI(int currentCount)
    {
        //In UpdateScore UI
        int remaining = Mathf.Max(0, collectibleData.GoalAmount - currentCount);
        collectedText.text = $"{currentCount}";
        remainingText.text = $"{remaining}";
    }

    // Updates the Countdown timer display each frame
    private void UpdateTimerUI(float remainingTime)
    {
        timerText.text = $"Time : {Mathf.Max(0, remainingTime):F1}";
    }
    // Shows You win on goal completed
    private void HandleGoalReached()
    {
        ShowEndPanel("You Win!");
    }

    // Shows Time's up when we are unable to achieve the goal in the time limit
    private void HandleTimeExpired()
    {
        ShowEndPanel("Time's Up!");
    }

    public void ShowEndPanel(string message)
    {
        endPanelText.text = message;
        SetPanel(endPanelCanvasGroup, true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    public void HideEndPanel()
    {
        SetPanel(endPanelCanvasGroup, false);
        Time.timeScale = 1f;
    }

    private void SetPanel(CanvasGroup group, bool isVisible)
    {
        group.alpha          = isVisible ? 1f : 0f;
        group.interactable   = isVisible;
        group.blocksRaycasts = isVisible;
    }
}