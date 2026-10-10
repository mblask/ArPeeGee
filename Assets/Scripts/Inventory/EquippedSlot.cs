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

    private Inventory _inventory;
    private PlayerEquipment _playerEquipment;
    private ItemTooltip _itemTooltip;
    private EquipmentManager _equipmentManager;

    public void OnEnable()
    {
        _inventory = Inventory.Instance;
        _itemTooltip = ItemTooltip.Instance;
        _playerEquipment = PlayerEquipment.Instance;
        _equipmentManager = EquipmentManager.Instance;
    }

    public bool IsEquipped
    {
        get
        {
            return _item != null;
        }
    }

    [SerializeField] private Equipment _item;

    public Equipment Item
    {
        get
        {
            return _item;
        }
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
        Debug.Log($"RemoveFromEquippedSlot, Item null {_item == null}");
        if (_item == null)
            return;

        if (_inventory == null)
        {
            Debug.Log("Cache inventory");
            _inventory = Inventory.Instance;
        }

        if (_inventory.IsFull())
        {
            _inventory.onInventoryFull?.Invoke();
            return;
        }

        Debug.Log("RemoveFromEquippedSlot, Inventory not full");
        
        if (_equipmentManager == null)
        {
            Debug.Log("Cache equipment manager");
            _equipmentManager = EquipmentManager.Instance;
        }

        //_inventory.AddItem(_item);
        _playerEquipment.RemoveItem(_item);
        //_equipmentManager.Unequip(_item);
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
            if (_itemTooltip == null)
                _itemTooltip = ItemTooltip.Instance;


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
