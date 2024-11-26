using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;          // Đối tượng mà camera sẽ theo dõi
    public Vector2 minPosition;       // Giới hạn bên trái và bên dưới của camera
    public Vector2 maxPosition;       // Giới hạn bên phải và trên của camera
    public float smoothSpeed = 0.125f; // Tốc độ di chuyển của camera

    void LateUpdate()
    {
        // Lấy vị trí của player và giới hạn trong khoảng min và max
        Vector3 targetPosition = new Vector3(
            Mathf.Clamp(player.position.x, minPosition.x, maxPosition.x),
            Mathf.Clamp(player.position.y, minPosition.y, maxPosition.y),
            transform.position.z
        );

        // Dịch chuyển camera một cách mượt mà
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}

