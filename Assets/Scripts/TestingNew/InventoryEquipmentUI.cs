using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryEquipmentUI : MonoBehaviour
{
    private static InventoryEquipmentUI _instance;
    public static InventoryEquipmentUI Instance
    {
        get
        {
            return _instance;
        }
    }

    public event Action<Equipment> OnWeaponChanged;
    public event Action<Equipment> OnWeaponRemoved;

    public event Action OnInventoryUIClose;

    private Transform _container;
    private InventorySlot[] _inventorySlots;
    private EquippedSlot[] _equipmentSlots;
    private Inventory _inventory;
    private PlayerEquipment _playerEquipment;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _container = transform.Find("Container");
        _inventorySlots = _container.Find("InventoryItems").GetComponentsInChildren<InventorySlot>();
        _equipmentSlots = _container.Find("EquippedItems").GetComponentsInChildren<EquippedSlot>();

        _inventory = Inventory.Instance;
        _playerEquipment = PlayerEquipment.Instance;

        UpdateInventoryUI();
        UpdateEquipmentUI();
        _inventory.onInventoryItemsChanged += UpdateInventoryUI;
        _playerEquipment.onEquipmentChanged += UpdateEquipmentUI;

        _container.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            bool activateUI = !_container.gameObject.activeSelf;

            if (!activateUI)
                OnInventoryUIClose?.Invoke();

            _container.gameObject.SetActive(activateUI);
        }

        if (Input.GetButtonDown("Cancel") && _container.gameObject.activeSelf)
        {
            _container.gameObject.SetActive(false);
        }
    }

    public void UpdateInventoryUI()
    {
        Debug.Log("UpdateInventoryUI");
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            if (i < _inventory.GetItems().Count)
            {
                _inventorySlots[i].AddItemToInventorySlot(_inventory.GetItems()[i]);
            }
            else
            {
                _inventorySlots[i].ClearInventorySlot();
            }
        }
    }

    public void UpdateEquipmentUI(List<EquipSlot> equipSlots = null)
    {
        if (equipSlots == null)
            return;

        for (int i = 0; i < _equipmentSlots.Length; i++)
        {
            Item item = equipSlots[i].GetItem();
            Equipment equipment = item as Equipment;

            //Removing the item from equipped slot
            if (equipment == null && _equipmentSlots[i].Item != null)
            {
                if (_equipmentSlots[i].Slot.Equals(EquipmentSlot.Weapon))
                    OnWeaponRemoved?.Invoke(_equipmentSlots[i].Item);

                _equipmentSlots[i].ClearSlot();
            }

            //Adding the item to equipped slot
            if (equipment != null)
            {
                _equipmentSlots[i].ClearSlot();
                _equipmentSlots[i].AddToEquippedSlot(equipment);

                if (equipment.EquipmentSlot.Equals(EquipmentSlot.Weapon))
                    OnWeaponChanged?.Invoke(equipment);
            }
        }
    }
}
