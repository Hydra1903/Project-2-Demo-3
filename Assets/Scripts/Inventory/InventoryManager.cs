using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class InventoryManager : MonoBehaviour
{
    [Header("Currency")]
    public int playerCurrency = 0;

    //[Header("InventoryPanel")]

    public GameObject itemSlotPrefab;
    public Transform itemSlotContainer;

    public List<Item> items = new List<Item>();
    private Item selectedItem;

    //private Dictionary<string, Item> item = new Dictionary<string, Item>();

    public static InventoryManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            //SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if(UIManager.instance != null)
        {
            UIManager.instance.UpdateCurrencyDisplay(UpdateCurrencyUI());
        }
    }
    private void OnItemClicked(Item item, Vector3 itemPosition)
    {
        selectedItem = item;

        if (InventoryUIManager.instance != null)
        {
            InventoryUIManager.instance.ShowItemInfo(item);
            InventoryUIManager.instance.actionMenu.SetActive(true);

            Vector3 offset = new Vector3(0, -90, 0);
            InventoryUIManager.instance.actionMenu.transform.position = itemPosition + offset;

            InventoryUIManager.instance.useButton.gameObject.SetActive(item.itemType == ItemType.Consumable);
        }
        else
        {
            Debug.LogWarning("InventoryUIManager instance not found");
        }
    }
    public void UpdateInventoryUI()
    {
        foreach (Transform child in itemSlotContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Item item in InventoryManager.instance.items)
        {
            GameObject slot = Instantiate(itemSlotPrefab,itemSlotContainer);
            slot.transform.Find("Icon").GetComponent<Image>().sprite = item.icon;
            slot.transform.Find("Quantity").GetComponent<Text>().text = $"x{item.quantity}";

            Button button = slot.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnItemClicked(item, slot.transform.position));
            }
        }
    }



    public void AddItem(Item newItem)
    {
        Item existingItem = items.Find(item => item.itemName == newItem.itemName);

        if (existingItem != null)
        {
            existingItem.quantity += newItem.quantity;
        }
        else
        {
            items.Add(newItem);
        }

        UpdateInventoryUI();
    }
    public void UseItem()
    {
        if (selectedItem != null && selectedItem.itemType == ItemType.Consumable)
        {
            selectedItem.quantity--;
            if (selectedItem.quantity <= 0)
            {
                items.Remove(selectedItem);
            }
            UpdateInventoryUI();
            InventoryUIManager.instance.CloseActionMenu();
        }
    }

    public void SellItem()
    {
        if (selectedItem != null && selectedItem.quantity > 0)
        {
            int totalPrice = selectedItem.price;
            AddCurrency(totalPrice);
            selectedItem.quantity--;
            if (selectedItem.quantity <= 0)
            {
                items.Remove(selectedItem);
                InventoryUIManager.instance.CloseActionMenu();
                selectedItem = null;
            }
            UpdateInventoryUI();
        }
    }
    // Hàm Equip Tool
    public void EquipTool()
    {
        if (selectedItem != null && selectedItem.itemType == ItemType.Tool)
        {
            ToolBarManager.instance.AddToolToToolBar(selectedItem);
            InventoryUIManager.instance.CloseActionMenu();
            Debug.Log("Tool đã được trang bị: " + selectedItem.itemName);
            UpdateInventoryUI();
        }
    }

    private string UpdateCurrencyUI()
    {
        return string.Format($"{playerCurrency:0}");
    }

    public void AddCurrency(int amount)
    {
        playerCurrency += amount;
        UpdateCurrencyUI();
    }

    // Các phương thức bổ sung để quản lý hạt giống
    public bool HasItem(string itemName)
    {
        Item item = items.Find(i => i.itemName == itemName);
        return item != null && item.quantity > 0;
    }

    public void RemoveItem(string itemName, int amount)
    {
        Item item = items.Find(i => i.itemName == itemName);
        if (item != null)
        {
            item.quantity -= amount;
            if (item.quantity <= 0)
            {
                items.Remove(item);
            }
            UpdateInventoryUI();
        }
    }

    public int GetItemQuantity(string itemName)
    {
        Item item = items.Find(i => i.itemName == itemName);
        return item != null ? item.quantity : 0;
    }  
}
