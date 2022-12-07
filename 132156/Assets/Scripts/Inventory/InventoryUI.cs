using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject InventoryUIGameObject;

    public Transform InventoryItems;
    public ItemTooltip ItemTooltip;

    private Inventory _inventory;
    private InventorySlot[] _slots;

    private GameObject _equippedItemsGameObject;

    private void Start()
    {
        _inventory = Inventory.Instance;
        if (_inventory != null)
            _inventory.onInventoryItemsChanged += UpdateUI;

        _slots = InventoryItems.GetComponentsInChildren<InventorySlot>();

        SetSlotIndices(_slots);

        InventoryUIGameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            InventoryUIGameObject.SetActive(!InventoryUIGameObject.activeSelf);
            if (!InventoryUIGameObject.activeSelf)
                ItemTooltip.HideItemTooltip();
        }

        if (Input.GetButtonDown("Cancel") && InventoryUIGameObject.activeSelf)
        {
            InventoryUIGameObject.SetActive(false);
            ItemTooltip.HideItemTooltip();
        }
    }

    private void SetSlotIndices(InventorySlot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Id = i;
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (i < _inventory._items.Count)
            {
                _slots[i].AddItemToInventorySlot(_inventory._items[i]);
            }
            else
            {
                _slots[i].ClearInventorySlot();
            }
        }
    }
}
