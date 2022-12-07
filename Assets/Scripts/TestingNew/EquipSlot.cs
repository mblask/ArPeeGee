using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipSlot
{
    public EquipmentSlot EquipmentSlot;

    [SerializeField] private Item _item;

    public void AddItem(Item item)
    {
        _item = item;
    }

    public void RemoveItem()
    {
        _item = null;
    }

    public Item GetItem()
    {
        return _item;
    }

    public bool IsEmpty()
    {
        return _item == null;
    }
}
