using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public delegate void OnEquipmentChanged(List<EquipSlot> equipSlots);
    public OnEquipmentChanged onEquipmentChanged;

    private static PlayerEquipment _instance;

    public static PlayerEquipment Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    private List<EquipSlot> _equipSlots = new List<EquipSlot>();

    private Inventory _inventory;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        EquippedSlot.onRemoveFromEquippedSlot += RemoveItem;
    }

    public void SetInventory(Inventory inventory)
    {
        _inventory = inventory;
    }

    public bool EquipItem(Item item)
    {
        Equipment equipment = item as Equipment;
        if (_inventory == null)
            _inventory = Inventory.Instance;

        int index = 0;
        foreach (EquipSlot slot in _equipSlots)
        {
            if (!slot.EquipmentSlot.Equals(equipment.EquipmentSlot))
                continue;

            index++;

            if (slot.IsEmpty())
            {
                Debug.Log($"Equip slot {slot.EquipmentSlot} is empty");
                slot.AddItem(item);
                _inventory.RemoveItem(item);
                onEquipmentChanged?.Invoke(_equipSlots);
                return true;
            }

            Debug.Log($"Equip slot {slot.EquipmentSlot} is populated");
            if (index < getNumOfSlots(slot.EquipmentSlot))
                continue;

            Item oldItem = slot.GetItem();
            slot.RemoveItem();
            slot.AddItem(item);
            _inventory.RemoveItem(item);
            
            if (oldItem != null)
                _inventory.AddItem(oldItem);
            
            onEquipmentChanged?.Invoke(_equipSlots);
            return false;
        }

        return false;
    }

    public void RemoveItem(Item item)
    {
        Debug.Log("PlayerEquipment, RemoveItem");
        foreach (EquipSlot slot in _equipSlots)
        {
            if (slot.IsEmpty())
                continue;

            if (!slot.GetItem().Equals(item))
                continue;

            slot.RemoveItem();
            onEquipmentChanged.Invoke(_equipSlots);

            bool addedToInventory = _inventory.AddItem(item);
            if (addedToInventory)
                return;
            
            EquipItem(item);
            return;
        }
    }

    private int getNumOfSlots(EquipmentSlot equipmentSlot)
    {
        int count = 0;

        foreach (EquipSlot slot in _equipSlots)
        {
            if (slot.EquipmentSlot.Equals(equipmentSlot))
                count++;
        }

        return count;
    }

    public List<EquipSlot> GetEquipSlots()
    {
        return _equipSlots;
    }
}
