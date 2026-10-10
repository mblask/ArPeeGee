using UnityEngine;

[System.Serializable]
public class EquipSlot
{
    public EquipmentSlot EquipmentSlot;

    [SerializeField] private Item _item;
    private bool _empty = true;

    public void AddItem(Item item)
    {
        _item = item;
        _empty = false;
    }

    public void RemoveItem()
    {
        _item = null;
        _empty = true;
    }

    public Item GetItem()
    {
        return _item;
    }

    public bool IsEmpty()
    {
        return _empty;
    }
}
