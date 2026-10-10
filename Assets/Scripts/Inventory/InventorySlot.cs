using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int Id;
    public Image Icon;
    public Button RemoveButton;

    [SerializeField] private Item _item;

    private ItemTooltip _itemTooltip;
    private Inventory _inventory;

    private void OnEnable()
    {
        _itemTooltip = ItemTooltip.Instance;
        _inventory = Inventory.Instance;
    }

    public void AddItemToInventorySlot(Item newItem)
    {
        Debug.Log($"AddItemToInventorySlot, newItem is null {newItem == null}");
        if (newItem == null)
            return;

        _item = newItem;

        Debug.Log($"AddItemToInventorySlot, item: {_item}");
        Debug.Log($"AddItemToInventorySlot, itemImage: {_item.ItemImage}");

        Icon.sprite = _item.ItemImage;
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
        _inventory.ThrowItem(_item);
    }

    public void UseItem()
    {
        if (_item == null)
            return;
        
        _item.Use();
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
        if (_itemTooltip == null)
            _itemTooltip = ItemTooltip.Instance;

        if (_item != null)
        {
            _itemTooltip.transform.position = pointerEventData.position;
            _itemTooltip.ShowItemTooltip(_item);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (_itemTooltip == null)
            _itemTooltip = ItemTooltip.Instance;

        _itemTooltip.HideItemTooltip();
    }
}
