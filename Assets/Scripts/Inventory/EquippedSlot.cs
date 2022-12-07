using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquippedSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void OnRemoveFromEquippedSlot(Item item);
    public static OnRemoveFromEquippedSlot onRemoveFromEquippedSlot;

    public Image Icon;
    public Sprite PlaceholderIcon;
    public EquipmentSlot Slot;

    public bool IsEquipped
    {
        get
        {
            return _item != null;
        }
    }

    [SerializeField] private Equipment _item;

    //[Header("Read-only")]
    //[SerializeField] private ItemTooltip _tooltip;

    public Equipment Item
    {
        get
        {
            return _item;
        }
    }

    //private void Awake()
    //{
    //    if (_tooltip == null)
    //        _tooltip = FindObjectOfType<ItemTooltip>();
    //}

    private void Start()
    {
        if (!IsEquipped)
        {
            Icon.sprite = PlaceholderIcon;
            Icon.color = ConstantContainer.StrongTransparency;
            Icon.preserveAspect = true;
        }
    }

    public void AddToEquippedSlot(Equipment newEquipment)
    {   
        _item = newEquipment;

        Icon.sprite = newEquipment.ItemImage;
        Icon.color = ConstantContainer.RegularSpriteColor;
        Icon.preserveAspect = true;
    }

    public void RemoveFromSlot()
    {
        Item itemToRemove = _item;
        ClearSlot();

        onRemoveFromEquippedSlot?.Invoke(itemToRemove);
    }

    public void RemoveFromEquippedSlot()
    {
        if (_item == null)
            return;

        if (Inventory.Instance.IsFull())
        {
            Inventory.Instance.onInventoryFull?.Invoke();
            return;
        }

        if (EquipmentManager.Instance != null)
            EquipmentManager.Instance.Unequip(_item);

        if (EquipmentManager.Instance != null)
        {
            Debug.Log("Remove " + _item.ItemName + " from slot");
            Inventory.Instance.AddItem(_item);
            EquipmentManager.Instance.onEquipmentChanged?.Invoke(null, _item);
        }

        ClearSlot();
    }

    public void ClearSlot()
    {
        _item = null;
        Icon.sprite = PlaceholderIcon;
        Icon.color = ConstantContainer.StrongTransparency;
        Icon.preserveAspect = true;
    }

    public Equipment GetEquipment()
    {
        return _item;
    }
    
    public bool IsEmpty()
    {
        return _item == null;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (_item != null)
        {
            ItemTooltip.Instance.transform.position = pointerEventData.position;
            ItemTooltip.Instance.ShowItemTooltip(_item);

            //_tooltip.transform.position = pointerEventData.position;
            //_tooltip.ShowItemTooltip(_item);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        ItemTooltip.Instance.HideItemTooltip();
        //_tooltip.HideItemTooltip();
    }
}
