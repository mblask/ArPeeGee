using System.Collections;
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

    [SerializeField] private List<EquipSlot> _equipSlots;

    private PlayerInventory _inventory;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        EquippedSlot.onRemoveFromEquippedSlot += RemoveItem;
    }

    public void SetInventory(PlayerInventory inventory)
    {
        _inventory = inventory;
    }

    public bool EquipItem(Item item)
    {
        Equipment equipment = item as Equipment;

        int index = 0;
        foreach (EquipSlot slot in _equipSlots)
        {
            if (slot.EquipmentSlot.Equals(equipment.EquipmentSlot))
            {
                index++;

                if (slot.IsEmpty())
                {
                    slot.AddItem(item);
                    _inventory.RemoveItem(item);
                    onEquipmentChanged?.Invoke(_equipSlots);
                    return true;
                }
                else
                {
                    if (index < getNumOfSlots(slot.EquipmentSlot))
                    {
                        continue;
                    }
                    else
                    {
                        Item oldItem = slot.GetItem();
                        slot.RemoveItem();
                        slot.AddItem(item);
                        _inventory.RemoveItem(item);
                        _inventory.AddItem(oldItem);
                        onEquipmentChanged?.Invoke(_equipSlots);
                        return false;
                    }
                }
            }
        }

        return false;
    }

    public void RemoveItem(Item item)
    {
        foreach (EquipSlot slot in _equipSlots)
        {
            if (slot.IsEmpty())
                continue;

            if (slot.GetItem().Equals(item))
            {
                slot.RemoveItem();

                bool addedToInventory = _inventory.AddItem(item);
                if (addedToInventory)
                {
                    return;
                }
                else
                {
                    EquipItem(item);
                    return;
                }
            }
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
