using UnityEngine;

/// <summary>
/// This script is Attached to every collectible in the scene.
/// Responsible only for detecting player collision and notifying the CollectionManager.
/// Intentionally knows nothing about game state or scoring logic.
/// </summary>

public class CollectibleBehaviour : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private CollectibleData data;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Guard against missing data assignment in the Inspector
        if (data == null)
        {
            Debug.LogWarning($"{gameObject.name} has no CollectibleData assigned.", this);
            return;
        }

        CollectionManager.Instance.CollectItem(data);

        // Deactivate instead of Destroy to keep this pool-friendly
        gameObject.SetActive(false);
    }
}