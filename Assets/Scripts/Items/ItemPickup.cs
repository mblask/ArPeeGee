using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable
{
    public Item Item;

    private void Start()
    {
        BringItemForward();
    }

    public override void Interact()
    {
        base.Interact();

        PickupItem();
    }

    public void PickupItem()
    {
        bool wasPickedUp = Inventory.Instance.AddItem(Item);

        if (wasPickedUp)
        {
            GameManager.Instance.HideItemNamePanel();
            GameManager.Instance.SetCursor(CursorType.Default);
            AudioManager.Instance.SFXAudioSource.PlayOneShot(AudioManager.Instance.ItemPickupAudioClip);
            Destroy(gameObject);
        }

    }

    public void BringItemForward()
    {
        Vector3 zPositionOffset = transform.position + Vector3.back * 0.1f;
        transform.position = zPositionOffset;
    }

    private void OnMouseOver()
    {
        if (gameObject != null)
        {
            GameManager.Instance.ShowItemNamePanel(Item.ItemName, transform);
        }
    }
    
    private void OnMouseExit()
    {
        GameManager.Instance.HideItemNamePanel();
    }
}
