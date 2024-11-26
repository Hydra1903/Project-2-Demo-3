using UnityEngine;

public class BasketArea : MonoBehaviour
{
    public GameObject panel;  // Tham chiếu tới Panel UI mà bạn muốn mở/tắt
    private bool isPlayerInCollider = false;  // Kiểm tra xem Player có trong Collider không
    private bool isPanelOpen = false;  // Kiểm tra trạng thái mở/tắt panel

    private void Start()
    {
        panel.SetActive(false);
    }

    // Khi Player vào vùng Collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInCollider = true;  // Player vào vùng Collider
            Debug.Log("Player đã vào vùng Collider của Basket.");  // Debug log khi Player vào Collider
        }
    }

    // Khi Player rời khỏi vùng Collider
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInCollider = false;  // Player ra khỏi vùng Collider
            Debug.Log("Player đã rời khỏi vùng Collider của Basket.");  // Debug log khi Player ra khỏi Collider

            if (isPanelOpen)
            {
                TogglePanel();  // Đóng panel nếu Player ra khỏi vùng Collider
            }
        }
    }

    // Cập nhật mỗi frame
    void Update()
    {
        // Kiểm tra nếu Player ở trong Collider và nhấn phím Q
        if (isPlayerInCollider && Input.GetKeyDown(KeyCode.Q))
        {
            TogglePanel();  // Mở hoặc tắt panel khi nhấn Q
        }
    }

    // Hàm mở hoặc đóng panel
    private void TogglePanel()
    {
        isPanelOpen = !isPanelOpen;  // Đảo trạng thái panel
        panel.SetActive(isPanelOpen);  // Kích hoạt hoặc tắt panel (vì Panel cũng là GameObject)
    }
}

