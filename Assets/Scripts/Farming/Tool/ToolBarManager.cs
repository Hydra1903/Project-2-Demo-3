using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBarManager : MonoBehaviour
{
    public static ToolBarManager instance;
    public List<Item> toolsInToolBar = new List<Item>();
    public GameObject toolSlotPrefab;
    public Transform toolBarContainer;

 
    public Player player;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddToolToToolBar(Item tool)
    {
        if (!toolsInToolBar.Contains(tool))
        {
            toolsInToolBar.Add(tool);
            UpdateToolBarUI();
        }
    }

    public void UpdateToolBarUI()
    {
        foreach (Transform child in toolBarContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Item tool in toolsInToolBar)
        {
            GameObject slot = Instantiate(toolSlotPrefab, toolBarContainer);
            slot.transform.Find("Icon").GetComponent<Image>().sprite = tool.icon;

            Button button = slot.GetComponent<Button>();
            //button.onClick.AddListener(() => SelectTool(tool));
            button.onClick.AddListener(() => ToggleToolInPlayer(tool));
        }
    }
    private void ToggleToolInPlayer(Item tool)
    {
        if (Player.instance != null)
        {
            Player.instance.ToggleTool(tool);  // Gọi ToggleTool từ Player để hiển thị hoặc ẩn Tool
        }
        else
        {
            Debug.LogWarning("Player instance không được thiết lập trong scene.");
        }
    }




    //private void SelectTool(Item tool)
    //{
    //    Debug.Log("Tool đang sử dụng: " + tool.itemName);
    //    player.EquipTool(tool);
    //}
}



