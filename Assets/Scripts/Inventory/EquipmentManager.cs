using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    #region Singleton
    public static EquipmentManager Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;
    }
    #endregion

    public Transform EquippedItems;
    public AudioClip EquipAudioClip;
    public AudioClip UnequipAudioClip;

    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    [SerializeField] private Equipment[] _currentEquipment;
    private Inventory _inventory;
    private EquippedSlot[] _equippedSlots;
    public Equipment[] CurrentEquipment => _currentEquipment;

    //private GameObject _equippedItems;
    //private const string EquippedItemsName = "EquippedItems";

    private int _numPlayerEquipSlots;

    private void Start()
    {
        _inventory = Inventory.Instance;

        _equippedSlots = EquippedItems.GetComponentsInChildren<EquippedSlot>();

        _numPlayerEquipSlots = _equippedSlots.Length;
        _currentEquipment = new Equipment[_numPlayerEquipSlots];
    }

    public bool Equip(Equipment newItem)
    {
        bool successfulEquip = false;

        int slotIndex = (int)newItem.EquipmentSlot;

        Equipment oldItem = null;

        //Counts how many equipment slots of this type of EquipmentSlot exist
        int itemSlotTotalCount = 0;
        foreach (EquippedSlot slot in _equippedSlots)
        {
            if (newItem.EquipmentSlot == slot.Slot)
                itemSlotTotalCount++;
        }

        //Sets the index of the first slot of a specific EquipmentSlot
        int currentSlotIndex = 0;
        for (int i = 0; i < _numPlayerEquipSlots; i++)
        {
            if (_equippedSlots[i].Slot != newItem.EquipmentSlot)
            {
                //Checking if the EquipmentSlot of the newItem agrees with the currently investigated _equippedSlot in the Inventory
                //Debug.Log("Inadequate slot");
                continue;
            }
            else
            {
                //If the EquipmentSlot is correct, proceed to check if the _equippedSlot is already equipped => filled with item
                //Debug.Log("Adequate slot found.");
                if (_equippedSlots[i].IsEquipped)
                {
                    //If it is filled and if the _equippedSlot is not the last of that type, continue the loop
                    if (currentSlotIndex < itemSlotTotalCount-1)
                    {
                        currentSlotIndex++;
                        continue;
                    }

                    if (_inventory.IsFull())
                    {
                        _inventory.onInventoryFull.Invoke();
                        return false;
                    }

                    //If it is filled and if the _equippedSlot IS the last of that type, replace the current item with newItem
                    //Debug.Log("Slot already used");
                    oldItem = _currentEquipment[i];
                    _currentEquipment[i] = newItem;
                    _equippedSlots[i].AddToEquippedSlot(newItem);
                    AudioManager.Instance.SFXAudioSource.PlayOneShot(EquipAudioClip);
                    successfulEquip = true;
                }
                else
                {
                    //If the _equippedSlot is empty, place newItem in it
                    //Debug.Log("Empty slot found. Slot " + _equipedSlots[i].Slot + "filled with " + newItem.ItemName);
                    oldItem = null;
                    _currentEquipment[i] = newItem;
                    _equippedSlots[i].AddToEquippedSlot(newItem);
                    AudioManager.Instance.SFXAudioSource.PlayOneShot(EquipAudioClip);

                    //Reset currentSlotIndex because we filled the slot
                    currentSlotIndex = 0;
                    successfulEquip = true;
                    break;
                }
            }
        }

        //If oldItem is not nothing, i.e. if there was something in the _equippedSlot before equipping newItem, place the oldItem back into the Inventory
        if (oldItem != null)
        {
            _inventory.AddItem(oldItem);
            //Debug.Log("Placing old item to the inventory");
        }

        //Debug.Log(onEquipmentChanged == null);

        if (onEquipmentChanged != null)
            onEquipmentChanged.Invoke(newItem, oldItem);

        return successfulEquip;
    }

    public void Unequip(int slotIndex)
    {
        if (_currentEquipment[slotIndex] != null)
        {
            Equipment oldItem = _currentEquipment[slotIndex];

            _inventory.AddItem(oldItem);
            
            _currentEquipment[slotIndex] = null;

            AudioManager.Instance.SFXAudioSource.PlayOneShot(UnequipAudioClip);

            if (onEquipmentChanged != null)
                onEquipmentChanged.Invoke(null, oldItem);
        }
    }

    public void Unequip(Equipment item)
    {
        if (item != null)
        {
            Equipment oldItem = item;

            _inventory.AddItem(oldItem);

            for (int i = 0; i < _currentEquipment.Length; i++)
            {
                if (_currentEquipment[i] == item)
                {
                    _currentEquipment[i] = null;
                    break;
                }
            }

            AudioManager.Instance.SFXAudioSource.PlayOneShot(UnequipAudioClip);

            if (onEquipmentChanged != null)
                onEquipmentChanged.Invoke(null, oldItem);
        }
    }

    public void UneqipAll()
    {
        for (int i = 0; i < _currentEquipment.Length; i++)
        {
            Unequip(i);
        }
    }   
}
