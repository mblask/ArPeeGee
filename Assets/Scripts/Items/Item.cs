using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName ="ArPeeGee/Items/Item", order = 1)]
public class Item : ScriptableObject
{
    [Header("Item Stats")]
    public string ItemName;
    public Sprite ItemImage;
    public GameObject ItemPrefab;
    [Range(0.0f, 100000f)]public float DropFrequency;

    public virtual void Use()
    {
        Debug.Log("Using " + ItemName);
    }

    public void RemoveFromInventory()
    {
        PlayerInventory.Instance.RemoveItem(this);
    }
}
