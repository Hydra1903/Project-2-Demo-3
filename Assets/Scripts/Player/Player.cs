using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    [SerializeField] private List<GameObject> toolObjects; // Các Tool đã tạo sẵn và là con của Player
    private GameObject currentToolObject; // Tool hiện tại đang được hiển thị
    public Item equippedTool; // Tool hiện tại được trang bị

    public ToolType currentTool = ToolType.None;  // Công cụ hiện tại
    public bool isToolActive = false;  // Trạng thái công cụ có đang active hay không
    public bool isUsingTool = false;

    //public List<Item> items = new List<Item>();
    //private SoitileManager cropManager;
   // public Animator anim;

    private void Awake()
    {
        //anim = GetComponent<Animator>();
        
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // Đảm bảo tất cả các Tool đều tắt khi khởi động
        foreach (GameObject tool in toolObjects)
        {
            tool.SetActive(false);
            Debug.Log("Tool in toolObjects: " + tool.name); // In ra tên Tool có trong danh sách
        }
    }
    private void Update()
    {
       
    }

    public void ToggleTool(Item tool)
    {
        // Nếu công cụ hiện tại đang là công cụ đã trang bị và đang hiển thị, thì tắt công cụ
        if (equippedTool == tool && currentToolObject != null)
        {
            currentToolObject.SetActive(false);  // Ẩn công cụ
            equippedTool = null;  // Đặt công cụ trang bị là null
            isUsingTool = false;
            currentToolObject = null;  // Đặt đối tượng tool hiện tại là null
            isToolActive = false;  // Tắt công cụ
            currentTool = ToolType.None;  // Đặt công cụ hiện tại về "None"
            Debug.Log("Công cụ đã bị tắt");
            return;
        }

        // Tắt công cụ hiện tại nếu có
        if (currentToolObject != null)
        {
            currentToolObject.SetActive(false);  // Ẩn công cụ hiện tại
        }

        // Gán công cụ mới
        equippedTool = tool;

        // Tìm GameObject tương ứng với công cụ trong danh sách
        currentToolObject = toolObjects.Find(t => t.name == tool.itemName);  // Kiểm tra tên công cụ trong danh sách toolObjects

        if (currentToolObject != null)
        {
            currentToolObject.SetActive(true);  // Hiển thị công cụ mới
            isUsingTool = true;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy GameObject Tool với tên: " + tool.itemName);
        }

        // Cập nhật công cụ và trạng thái active
        if (tool != null && tool.itemType == ItemType.Tool)
        {
            currentTool = tool.toolType;  // Cập nhật công cụ hiện tại từ toolType
            isToolActive = true;  // Đảm bảo công cụ đang active
            Debug.Log("Công cụ hiện tại: " + currentTool + ", Trạng thái active: " + isToolActive);
        }
        else
        {
            currentTool = ToolType.None;  // Nếu không có công cụ, đặt công cụ về None
            isToolActive = false;
            Debug.LogWarning("itemType của công cụ không phải là Tool hoặc không hợp lệ");
        }
    }

    // Hàm để tắt công cụ
    public void DeactivateTool()
    {
        isToolActive = false;  // Tắt công cụ
        if (currentToolObject != null)
        {
            currentToolObject.SetActive(false);
            currentToolObject = null;
        }
        currentTool = ToolType.None;
        equippedTool = null;
        Debug.Log("Công cụ đã bị tắt");
    }

    public bool CanDig()
    {
        return currentTool == ToolType.Shovel && isToolActive;  // Kiểm tra nếu công cụ là Shovel và active
    }

    public bool CanPlantSeeds()
    {
        return currentTool == ToolType.SeedBag && isToolActive;  // Kiểm tra nếu công cụ là SeedBag và active
    }
    public bool CanWatering()
    {
        return currentTool == ToolType.WateringCan && isToolActive;
    }
    public bool CanHarvest()
    {
        return currentTool == ToolType.HandHarvest && isToolActive;
    }
    public bool CanFishing()
    {
        return currentTool == ToolType.FishingRod && isToolActive;
    }
    //public void IsWatering()
    //{
    //    anim.SetTrigger("watering");
    //}
}







