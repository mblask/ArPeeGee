﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static Inventory _instance;

    public static Inventory Instance
    {
        get
        {
            return _instance;
        }
    }

    public int Size = 12;
    public List<Item> _items;
    public float ThrowDistance = 0.0f;

    public delegate void OnInventoryItemsChanged();
    public OnInventoryItemsChanged onInventoryItemsChanged;

    public delegate void OnInventoryFull();
    public OnInventoryFull onInventoryFull;

    public delegate void OnNoPotions();
    public OnNoPotions onNoPotions;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        if (onInventoryItemsChanged != null)
            onInventoryItemsChanged.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ConsumePotion(PotionType.Health);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ConsumePotion(PotionType.Magic);
        }
    }

    public bool AddItem(Item item)
    {
        if (IsFull())
        {
            if (onInventoryFull != null)
                onInventoryFull.Invoke();

            return false;
        }

        _items.Add(item);

        if (onInventoryItemsChanged != null)
            onInventoryItemsChanged.Invoke();

        return true;
    }

    public void RemoveItem(Item item)
    {
        _items.Remove(item);

        if (onInventoryItemsChanged != null)
            onInventoryItemsChanged.Invoke();
    }

    public void RemoveItem(int inventoryPosition)
    {
        RemoveItem(_items[inventoryPosition]);
    }

    public void RemoveAllItems()
    {
        _items.Clear();

        if (onInventoryItemsChanged != null)
            onInventoryItemsChanged.Invoke();
    }

    public void ThrowItem(Item item)
    {
        if (item == null)
            return;

        Vector2 dropPosition = (Vector2)transform.position + Vector2.down * ThrowDistance;
        GameObject thrownItem = Instantiate(item.ItemPrefab, dropPosition, Quaternion.identity);
        /**/
        thrownItem.GetComponent<ItemPickup>().Item = item;
        thrownItem.GetComponent<SpriteRenderer>().sprite = item.ItemImage;
        /**/

        RemoveItem(item);
    }

    public void ConsumePotion(PotionType type)
    {
        Consumable potion = null;
        foreach (Item item in _items)
        {
            if (item is Consumable && item.ItemName.Contains(type.ToString()))
            {
                potion = (Consumable)item;
                potion.Use();
                break;
            }
        }

        if (potion == null)
        {
            if (onNoPotions != null)
                onNoPotions.Invoke();
        }
    }

    public bool IsFull()
    {
        return _items.Count >= Size;
    }

    public List<Item> GetItems()
    {
        return _items;
    }
}
