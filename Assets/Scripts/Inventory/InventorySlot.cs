using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int Id;
    public Image Icon;
    public Button RemoveButton;

    [SerializeField] private Item _item;

    //[SerializeField] private ItemTooltip _tooltip;

    //private void Awake()
    //{
    //    if (_tooltip == null)
    //        _tooltip = FindObjectOfType<ItemTooltip>();
    //}

    public void AddItemToInventorySlot(Item newItem)
    {
        _item = newItem;

        Icon.sprite = newItem.ItemImage;
        Icon.enabled = true;
        Icon.preserveAspect = true;

        RemoveButton.interactable = true;
    }

    public void ClearInventorySlot()
    {
        _item = null;

        Icon.sprite = null;
        Icon.enabled = false;

        RemoveButton.interactable = false;
    }

    public void OnRemoveButton()
    {
        Inventory.Instance.ThrowItem(_item);
    }

    public void UseItem()
    {
        if (_item != null)
        {
            _item.Use();
        }
    }

    public Item GetItem()
    {
        return _item;
    }

    public bool IsEmpty()
    {
        return _item == null;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (_item != null/* && _item is Equipment*/)
        {
            ItemTooltip.Instance.transform.position = pointerEventData.position;
            ItemTooltip.Instance.ShowItemTooltip(_item);

            //_tooltip.transform.position = pointerEventData.position;
            //_tooltip.ShowItemTooltip(/*(Equipment)*/_item);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        ItemTooltip.Instance.HideItemTooltip();
        //_tooltip.HideItemTooltip();
    }
}
