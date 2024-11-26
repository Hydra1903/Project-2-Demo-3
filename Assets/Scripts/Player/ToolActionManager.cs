using UnityEngine;

public class ToolActionManager : MonoBehaviour
{
    public ToolType currentTool;  // Công cụ hiện tại
    private string currentArea;   // Khu vực hiện tại



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            PerformToolAction();
        }
    }

    private void PerformToolAction()
    {
        switch (currentTool)
        {
            case ToolType.FishingRod:
                if (currentArea == "FishingSpot")
                {
                    Debug.Log("Đang thực hiện animation câu cá: Ném cần");
                    // Kích hoạt animation Ném cần câu
                }
                else
                {
                    Debug.Log("Không thể câu cá ở khu vực này.");
                }
                break;

            case ToolType.WateringCan:
                if (currentArea == "CropArea")
                {
                    Debug.Log("Đang thực hiện animation tưới cây");
                    // Kích hoạt animation tưới cây
                }
                else
                {
                    Debug.Log("Chỉ có thể dùng bình tưới trong khu vực trồng cây.");
                }
                break;

            case ToolType.Shovel:
                if (currentArea == "CropArea")
                {
                    Debug.Log("Đang thực hiện animation đào đất");
                    // Kích hoạt animation đào đất
                 
                }
                else
                {
                    Debug.Log("Không thể đào ở khu vực này.");
                }
                break;

            // Thêm các case cho các công cụ khác nếu cần
            default:
                Debug.Log("Không có hành động nào được định nghĩa cho công cụ này.");
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FishingSpot"))
        {
            currentArea = "FishingSpot";
        }
        else if (other.CompareTag("CropArea"))
        {
            currentArea = "CropArea";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        currentArea = null; // Reset khi rời khỏi khu vực
    }
}

