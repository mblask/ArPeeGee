using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquippedSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
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

    private Equipment _item;

    [Header("Read-only")]
    [SerializeField] private ItemTooltip _tooltip;

    public Equipment Item
    {
        get
        {
            return _item;
        }
    }

    private void Awake()
    {
        if (_tooltip == null)
            _tooltip = FindObjectOfType<ItemTooltip>();
    }

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

    public void RemoveFromEquippedSlot()
    {
        if (_item == null)
            return;

        if (Inventory.Instance.IsFull)
        {
            Inventory.Instance.onInventoryFull.Invoke();
            return;
        }

        EquipmentManager.Instance.Unequip(_item);

        _item = null;
        Icon.sprite = PlaceholderIcon;
        Icon.color = ConstantContainer.StrongTransparency;
        Icon.preserveAspect = true;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (_item != null)
        {
            _tooltip.transform.position = pointerEventData.position;
            _tooltip.ShowItemTooltip(_item);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        _tooltip.HideItemTooltip();
    }
}
