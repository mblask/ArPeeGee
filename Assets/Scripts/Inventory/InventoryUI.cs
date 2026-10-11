using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject InventoryUIGameObject;
    private Transform _container;
    private bool _containerVisible;

    public Transform InventoryItems;
    private ItemTooltip _itemTooltip;

    private Inventory _inventory;
    private InventorySlot[] _slots;

    private void Awake()
    {
        _container = transform.Find("InventoryUI").Find("Container");
        _containerVisible = false;
    }

    private void Start()
    {
        _inventory = Inventory.Instance;
        Debug.Log($"InventoryUI, inventory null: {_inventory == null}");
        if (_inventory != null)
            _inventory.onInventoryItemsChanged += UpdateUI;

        _itemTooltip = ItemTooltip.Instance;

        _slots = InventoryItems.GetComponentsInChildren<InventorySlot>();
        SetSlotIndices(_slots);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            _containerVisible = !_containerVisible;

            Debug.Log("Button: Inventory");
            ShowContainer(_containerVisible);
            if (_itemTooltip == null)
                _itemTooltip = ItemTooltip.Instance;

            if (!_container.gameObject.activeSelf)
                _itemTooltip.HideItemTooltip();
        }

        if (Input.GetButtonDown("Cancel") && _container.gameObject.activeSelf)
        {
            ShowContainer(false);
            if (_itemTooltip == null)
                _itemTooltip = ItemTooltip.Instance;

            _itemTooltip.HideItemTooltip();
        }
    }

    private void ShowContainer(bool value)
    {
        _container.gameObject.SetActive(value);
    }

    private void SetSlotIndices(InventorySlot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].Id = i;
    }

    public void UpdateUI()
    {
        Debug.Log("UpdateUI");
        Debug.Log($"Inventory items: {_inventory.GetItems().Count}");
        List<Item> inventoryItems = _inventory.GetItems();
        for (int i = 0; i < _slots.Length; i++)
        {
            if (i < inventoryItems.Count)
            {
                Item item = inventoryItems[i];
                Debug.Log($"Item number {i}, item: {item}");
                _slots[i].AddItemToInventorySlot(item);
            }
            else
                _slots[i].ClearInventorySlot();
        }
    }
}
