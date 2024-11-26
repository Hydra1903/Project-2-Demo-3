using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedSelectionUIManager : MonoBehaviour
{
    public List<Button> seedButtons;
    //public Button carrotButton;
    //public Button tomatoButton;
    //public Text quantityText;
    private InventoryManager inventoryManager;
    
    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        //// Gắn sự kiện nhấp chuột vào từng nút
        //carrotButton.onClick.AddListener(() => SelectSeed("Carrot Seed"));
        //tomatoButton.onClick.AddListener(() => SelectSeed("Tomato Seed"));
        // Gắn sự kiện cho tất cả các nút trong danh sách seedButtons
        foreach (Button button in seedButtons)
        {
            string seedName = button.name; // Lấy tên hạt giống từ tên của button
            button.onClick.AddListener(() => SelectSeed(seedName));
        }

    }
  
    void SelectSeed(string seedName)
    {
        SoitileManager soilManager = FindObjectOfType<SoitileManager>();
        if (soilManager != null)
        {
            soilManager.SelectSeed(seedName);
            Debug.Log("Đã chọn hạt giống: " + seedName);
        }
        else
        {
            Debug.LogError("Không tìm thấy SoitileManager trong scene!");
        }
    }
   
}

