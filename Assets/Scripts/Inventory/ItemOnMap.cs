using UnityEngine;

public class ItemOnMap : MonoBehaviour
{
    public string itemName;       // Tên của Item
    public Sprite icon;           // Icon của Item
    public int quantity = 1;      // Số lượng Item trên bản đồ
    public ItemType itemType;     // Loại Item (Consumable hoặc Quest)
    public ToolType toolType;
    public int price;             // Giá của Item
    public int energy;            // Năng lượng phục hồi nếu là Consumable
    public string description;    // Mô tả của Item
    //public GameObject iconPickUp;

    private void Start()
    {
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu Player nhặt Item
        if (collision.CompareTag("Player"))
        {
           
            // Tạo một Item mới dựa trên thông tin của Item trên bản đồ
            Item newItem = new Item(itemName, icon, quantity, itemType, toolType ,price, energy, description);

            // Gọi InventoryManager để thêm Item vào Inventory
            InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
            if (inventoryManager != null)
            {
                inventoryManager.AddItem(newItem);
                Debug.Log($"Đã thêm {newItem.itemName} vào Inventory.");
            }

            // Hủy bỏ ItemOnMap sau khi nhặt
            Destroy(gameObject);
        }
    }
}




