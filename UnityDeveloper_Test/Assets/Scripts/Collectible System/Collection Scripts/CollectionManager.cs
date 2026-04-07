using UnityEngine;
using System;

/// <summary>
/// This is the Central manager for the collection system.
/// Tracks collected item count, runs the countdown timer, and broadcasts events for other systems to react to.
/// Follows a Singleton pattern for global access.
/// </summary>

// Attach this component to a dedicated empty GameObject in the scene.
// Do not attach alongside other managers to keep responsibilities separate.

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance { get; private set; }

    [Header("Data")]
    [SerializeField] private CollectibleData collectibleData;
    
    /// <summary> Fired on every successful collection. Passes current total count. </summary>
    public static event Action<int> OnItemCollected;

    /// <summary> Fired when collected count meets or exceeds the goal amount. </summary>
    public static event Action OnGoalReached;

    /// <summary> Fired when the countdown timer reaches zero. </summary>
    public static event Action OnTimeExpired;

    /// <summary> Fired every frame while timer is active. Passes remaining time. </summary>
    public static event Action<float> OnTimerUpdated;

    private int   currentCount    = 0;
    private float remainingTime;
    private bool  isTimerRunning  = false;
    private bool  goalReached     = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (collectibleData == null)
        {
            Debug.LogError("CollectionManager has no CollectibleData assigned. Please assign it in the Inspector.", this);
            return;
        }

        remainingTime = collectibleData.TimeLimit;
    }

    private void Update()
    {
        if (!isTimerRunning || goalReached) return;

        remainingTime -= Time.deltaTime;
        OnTimerUpdated?.Invoke(remainingTime);

        if (remainingTime <= 0f)
        {
            remainingTime  = 0f;
            isTimerRunning = false;
            OnTimeExpired?.Invoke();
        }
    }

    public void StartGame()
    {
        isTimerRunning = true;
    }

    /// <summary>
    /// Called by CollectibleBehaviour when player collides with a collectible.
    /// Adds item's point value to the running total and checks against the goal.
    /// </summary>
    /// <param name="data"> The ScriptableObject data of the collected item. </param>
    
    public void CollectItem(CollectibleData data)
    {
        if (!isTimerRunning || goalReached) return;

        currentCount += data.PointValue;
        OnItemCollected?.Invoke(currentCount);

        if (currentCount >= collectibleData.GoalAmount)
        {
            goalReached    = true;
            isTimerRunning = false;
            OnGoalReached?.Invoke();
        }
    }
}