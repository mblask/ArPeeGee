using UnityEngine;

[System.Serializable]
public struct ItemStruct
{
    public Item Item;
    public int Amount;
    public float Durability;

    public ItemStruct(Item item, int amount = 1)
    {
        Item = item;
        Amount = amount;
        Durability = Random.Range(20.0f, 50.0f);
    }
}
