using UnityEngine;

public class FishingSpot : MonoBehaviour
{
    public Transform fishingSpot; // Vị trí câu cá
    public float detectionRadius = 2f; // Bán kính phát hiện khu vực nước

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(fishingSpot.position, detectionRadius);
    }
}

