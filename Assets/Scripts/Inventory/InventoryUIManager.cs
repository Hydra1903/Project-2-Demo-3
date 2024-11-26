using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class InventoryUIManager : MonoBehaviour
{
    public Item selectedItem;
    public static InventoryUIManager instance;
    [Header("Inventory UI Elements")]
    public GameObject inventoryPanel;
    public GameObject actionMenu;
    public GameObject itemInfoPanel;

    public Button openInventory;
    public Button closeInventory;
    public Button useButton;
    public Button sellButton;
    public Button exitButton;
    public Button equipButton;

    [Header("Item Information UI")]
    public Text itemNameText;
    public Text itemDescriptionText;
    public Text itemPriceText;
    public Text itemEnergyText;
    public Image itemIcon;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Giữ lại Canvas khi chuyển Scene
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Đảm bảo chỉ có một instance của InventoryUIManager
        }
    }

    private void Start()
    {
        ConfigureButtons(); // Thiết lập các hành động cho nút
        inventoryPanel.SetActive(false);
        actionMenu.SetActive(false);
        itemInfoPanel.SetActive(false);
    }
    private void Update()
    {
        // Kiểm tra và gán lại nếu tham chiếu bị mất
        CheckAndAssignReferences();
    }

    // Tìm lại các tham chiếu UI khi chuyển Scene


    // Tìm các thành phần UI theo tên và kiểm tra nếu chúng có tồn tại trong Scene mới
    private void CheckAndAssignReferences()
    {
        // Chỉ tìm kiếm lại tham chiếu nếu chúng bị mất
        if (inventoryPanel == null) inventoryPanel = GameObject.Find("InventoryPanel");
        if (actionMenu == null) actionMenu = GameObject.Find("ActionMenu");
        if (itemInfoPanel == null) itemInfoPanel = GameObject.Find("ItemInfoPanel");

        if (openInventory == null) openInventory = GameObject.Find("OpenInventoryButton")?.GetComponent<Button>();
        if (closeInventory == null) closeInventory = GameObject.Find("CloseInventoryButton")?.GetComponent<Button>();
        if (useButton == null) useButton = GameObject.Find("UseButton")?.GetComponent<Button>();
        if (sellButton == null) sellButton = GameObject.Find("SellButton")?.GetComponent<Button>();
        if (exitButton == null) exitButton = GameObject.Find("ExitButton")?.GetComponent<Button>();
        if (equipButton == null) exitButton = GameObject.Find("EquipButton")?.GetComponent<Button>();

        if (itemNameText == null) itemNameText = GameObject.Find("ItemNameText")?.GetComponent<Text>();
        if (itemDescriptionText == null) itemDescriptionText = GameObject.Find("ItemDescriptionText")?.GetComponent<Text>();
        if (itemPriceText == null) itemPriceText = GameObject.Find("ItemPriceText")?.GetComponent<Text>();
        if (itemEnergyText == null) itemEnergyText = GameObject.Find("ItemEnergyText")?.GetComponent<Text>();
        if (itemIcon == null) itemIcon = GameObject.Find("ItemIcon")?.GetComponent<Image>();

        ConfigureButtons(); // Đảm bảo các sự kiện nút được gán đúng
    }

    private void ConfigureButtons()
    {
        openInventory?.onClick.RemoveAllListeners();
        openInventory?.onClick.AddListener(ToggleInventory);

        closeInventory?.onClick.RemoveAllListeners();
        closeInventory?.onClick.AddListener(CloseInventory);

        useButton?.onClick.RemoveAllListeners();
        useButton?.onClick.AddListener(InventoryManager.instance.UseItem);

        sellButton?.onClick.RemoveAllListeners();
        sellButton?.onClick.AddListener(InventoryManager.instance.SellItem);
        
        equipButton?.onClick.RemoveAllListeners();
        equipButton?.onClick.AddListener(InventoryManager.instance.EquipTool);

        exitButton?.onClick.RemoveAllListeners();
        exitButton?.onClick.AddListener(CloseActionMenu);
    }

    public void ToggleInventory()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    public void CloseInventory()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    // Hiển thị thông tin của Item đã chọn
    public void ShowItemInfo(Item item)
    {
        if (itemInfoPanel != null)
        {
            itemInfoPanel.SetActive(true);
            itemNameText.text = item.itemName;
            itemDescriptionText.text = item.description;
            itemPriceText.text = $"Price: {item.price}";
            itemEnergyText.text = item.itemType == ItemType.Consumable ? $"Energy: {item.energy}" : "";
            itemIcon.sprite = item.icon;
        }
    }

    // Đóng Menu Hành Động
    public void CloseActionMenu()
    {
        if (actionMenu != null)
            actionMenu.SetActive(false);
        if (itemInfoPanel != null)
            itemInfoPanel.SetActive(false);
    }
}




