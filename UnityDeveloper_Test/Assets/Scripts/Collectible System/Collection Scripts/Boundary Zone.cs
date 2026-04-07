using UnityEngine;

/// <summary>
/// Detects when the player falls out of the playable area.
/// Attach this to an invisible trigger volume placed beneath the map.
/// Notifies the UIManager to display the end panel with an "Out of Bounds" message.
/// </summary>

public class BoundaryZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // FindFirstObjectByType is acceptable here since this is a one-time event call,
        // not a per-frame operation — no performance concern.
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        uiManager.ShowEndPanel("Out of Bounds!");
    }
}