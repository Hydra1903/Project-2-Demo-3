using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopPanel : MonoBehaviour
{
    public static ShopPanel instance;
    [Header("UI References")]
    public GameObject itemSlotPrefab; // Prefab của ItemSlot
    public Transform itemSlotContainer; // Container chứa GridLayout Group
    public Text totalCurrencyText; // Hiển thị số tiền của Player
    public GameObject itemInfoPanel;
    public GameObject shopPanel;

    public Text itemStockText; // Thêm vào UI để hiển thị stock

    [Header("Item Information UI")]
    public Text itemNameText;
    public Text itemDescriptionText;
    public Text itemPriceText;
    public Image itemIcon;

    [Header("Button")]
    public Button buyButton;
    public Button closeButton;

    [Header("Shop Data")]
    public List<ShopItem> shopItems = new List<ShopItem>(); // Danh sách Item được bán
    public InventoryManager playerInventory; // Inventory của Player

    private ShopItem selectedItem;
    [System.Serializable]
    public class ShopItem
    {
        public Sprite itemIcon; // Hình ảnh Item
        public string itemName; // Tên Item
        public string description;
        public int price; // Giá Item
        public int amount = 1; // Số lượng mặc định
        public GameObject itemPrefab; // Prefab của Item để thêm vào Inventory

        public int maxStockPerDay = 10; // Giới hạn số lượng mỗi ngày
        [HideInInspector] public int remainingStock; // Số lượng còn lại trong ngày
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this; // Gán instance nếu chưa có
        }
        else
        {
            Destroy(gameObject); // Đảm bảo chỉ có một ShopPanel tồn tại
        }
    }
    public void ResetDailyStock()
    {
        foreach (var shopItem in shopItems)
        {
            shopItem.remainingStock = shopItem.maxStockPerDay;
            // Cập nhật UI cho ItemSlot
            foreach (Transform child in itemSlotContainer)
            {
                Text stockText = child.Find("StockText").GetComponent<Text>();
                if (child.Find("ItemIcon").GetComponent<Image>().sprite == shopItem.itemIcon)
                {
                    stockText.text = $"Stock: {shopItem.remainingStock}";
                    break;
                }
            }
        }
    }

    private void Start()
    {
        ResetDailyStock(); // Reset stock khi bắt đầu game
        itemInfoPanel.SetActive(false);
        shopPanel.SetActive(false);
        PopulateShop(); // Tạo các ItemSlot từ danh sách shopItems
                        
        buyButton.onClick.AddListener(BuySelectedItem);// Đăng ký sự kiện cho nút BuyButton
        closeButton.onClick.AddListener(CloseShopPanel);
    }
 
    // Tạo các ô ItemSlot và hiển thị thông tin từ danh sách shopItems
    private void PopulateShop()
    {
        foreach (var shopItem in shopItems)
        {
            // Tạo một ItemSlot từ prefab
            GameObject itemSlotInstance = Instantiate(itemSlotPrefab, itemSlotContainer);

            // Thiết lập thông tin hiển thị
            itemSlotInstance.transform.Find("ItemIcon").GetComponent<Image>().sprite = shopItem.itemIcon;

            //itemSlotInstance.transform.Find("StockText").GetComponent<Text>().text = shopItem.remainingStock.ToString();
            // Hiển thị số lượng stockRemaining
            Text stockText = itemSlotInstance.transform.Find("StockText").GetComponent<Text>();
            stockText.text = $"Stock: {shopItem.remainingStock}";


            // Gắn sự kiện nhấp chuột để hiển thị ItemInfoPanel
            Button itemButton = itemSlotInstance.GetComponent<Button>();
            itemButton.onClick.AddListener(() => ShowItemInfo(shopItem));
        }
    }
    public void ShowItemInfo(ShopItem item)
    {
        // Nếu đã mở ItemInfoPanel và nhấn vào cùng Item đang hiển thị -> ẩn đi
        if (itemInfoPanel.activeSelf && selectedItem == item)
        {
            itemInfoPanel.SetActive(false); // Ẩn ItemInfoPanel
            selectedItem = null; // Reset Item được chọn
            return;
        }
        selectedItem = item; // Lưu lại Item đang được chọn

            itemInfoPanel.SetActive(true);
            itemNameText.text = item.itemName;
            itemDescriptionText.text = item.description;
            itemPriceText.text = $"Price: {item.price}";
            itemStockText.text = $"Stock: {item.remainingStock}/{item.maxStockPerDay}";
            itemIcon.sprite = item.itemIcon;
            totalCurrencyText.text = $"{item.price}";
    }
    private void BuySelectedItem()
    {
        if (selectedItem == null)
        {
            Debug.LogWarning("Không có Item nào được chọn để mua!");
            return;
        }
        if (selectedItem.remainingStock <= 0)
        {
            Debug.LogWarning($"Không còn đủ hàng cho {selectedItem.itemName} hôm nay!");
            return;
        }


        // Tính tổng giá dựa trên số lượng
        //int amountToBuy = int.Parse(infoAmountInput.text);
        int totalPrice = selectedItem.price; //* amountToBuy;

        // Kiểm tra Player có đủ tiền không
        if (playerInventory.playerCurrency >= totalPrice)
        {
            // Trừ tiền Player
            playerInventory.playerCurrency -= totalPrice;
            ItemOnMap itemPrefab = selectedItem.itemPrefab.GetComponent<ItemOnMap>();

            Item newItem = new Item(
                itemPrefab.itemName,
                itemPrefab.icon,
                itemPrefab.quantity,
                itemPrefab.itemType,
                itemPrefab.toolType,
                itemPrefab.price,
                itemPrefab.energy,
                itemPrefab.description
            );

            // Thêm Item vào Inventory
            playerInventory.AddItem(newItem);

            // Giảm stock
            selectedItem.remainingStock--;
            // Sau khi giảm stock, cập nhật lại Text trong ItemSlot
            foreach (Transform child in itemSlotContainer)
            {
                Text stockText = child.Find("StockText").GetComponent<Text>();
                if (child.Find("ItemIcon").GetComponent<Image>().sprite == selectedItem.itemIcon)
                {
                    stockText.text = $"Stock: {selectedItem.remainingStock}";
                    break;
                }
            }

            Debug.Log($"Đã mua {itemPrefab.itemName} với giá {totalPrice}. Tiền còn lại: {playerInventory.playerCurrency}. Số lượng còn lại: {selectedItem.remainingStock}");

            // Tắt ItemInfoPanel sau khi mua
            itemInfoPanel.SetActive(false);
            totalCurrencyText.text = $"0";
            selectedItem = null;
        }
        else
        {
            Debug.LogWarning("Không đủ tiền để mua Item này!");
        }
    }
    public void CloseShopPanel()
    {
        shopPanel.SetActive(false);
    }

}


