using UnityEngine;

/// <summary>
/// This is a ScriptableObject that defines a collectible item's properties.
/// Create new item types via Assets → Create → Collection System → Collectible Data.
/// No code changes needed to add new collectible types.
/// </summary>

[CreateAssetMenu(fileName = "CollectibleData", menuName = "Collection System/Collectible Data")]
public class CollectibleData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string itemName = "Coin";
    [SerializeField] private int pointValue = 1;

    [Header("Goal Settings")]
    [SerializeField] private int goalAmount = 10;
    [SerializeField] private float timeLimit = 30f;

    // Public getters — read only access for other scripts
    public string ItemName => itemName;
    public int PointValue => pointValue;
    public int GoalAmount => goalAmount;
    public float TimeLimit => timeLimit;

}