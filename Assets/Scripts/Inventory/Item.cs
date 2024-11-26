using UnityEngine;
using UnityEngine.Tilemaps;

public enum ItemType
{
    Consumable, // Có thể dùng để hồi phục năng lượng, sức khỏe, v.v.
    Quest, // Chỉ dùng cho nhiệm vụ, không thể sử dụng
    Tool,
    Seed,
    Fish
}
public enum ToolType
{
    None,        // Không phải Tool
    FishingRod,  // Cần câu
    Shovel,      // Xẻng
    Axe,         // Rìu
    WateringCan,  // Bình tưới
    SeedBag,       // Bịch hạt giống
    HandHarvest
}

[System.Serializable]
public class Item 
{
    public string itemName;
    public Sprite icon;
    public int quantity;
    public ItemType itemType; // Loại Item
    public ToolType toolType; // Loại Tool nếu là Tool
    public int price; // Giá bán của Item
    public int energy; // Năng lượng phục hồi nếu là Consumable
    public string description; // Mô tả chi tiết Item

    public GameObject toolPrefab; // Prefab để hiển thị Tool
    public Item(string name, Sprite icon, int quantity, ItemType itemType,ToolType toolType ,int price, int energy, string description, GameObject prefab = null)
    {
        this.itemName = name;
        this.icon = icon;
        this.quantity = quantity;
        this.itemType = itemType;
        this.price = price;
        this.energy = energy;
        this.description = description;
        this.toolPrefab = prefab;
        this.toolType = toolType;
    }
}





