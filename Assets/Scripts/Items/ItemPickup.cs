using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable
{
    public Item Item;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        BringItemForward();
    }

    public override void Interact()
    {
        base.Interact();

        PickupItem();
    }

    public void PickupItem()
    {
        bool wasPickedUp = PlayerInventory.Instance.AddItem(Item);

        if (wasPickedUp)
        {
            ItemNamePanel.Instance?.ShowItemNamePanel(false);
            GameManager.Instance?.SetCursor(CursorType.Default);

            if (AudioManager.Instance != null)
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
        ItemNamePanel.Instance?.SetupPanel(_camera.WorldToScreenPoint(transform.position), Item.ItemName);
    }
    
    private void OnMouseExit()
    {
        ItemNamePanel.Instance?.ShowItemNamePanel(false);
    }
}
