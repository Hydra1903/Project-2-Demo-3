using UnityEngine;
public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
}
[CreateAssetMenu(fileName = "FishData", menuName = "Inventory/FishData")]
public class FishData : ScriptableObject
{
    public string fishName;
    public Sprite icon;
    public int quantity;
    public ItemType itemType;
    public ToolType toolType;
    public int price;
    public int energy;
    public string description;

    [Range(0, 100)]
    public float catchChance; // Tỷ lệ phần trăm câu trúng cá (0 - 100)

    public Rarity rarity;
}


