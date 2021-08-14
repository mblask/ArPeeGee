using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    public Sprite ChestOpen;
    public AudioClip OpenChestAudioClip;

    private SpriteRenderer _chestSpriteRenderer;
    private ParticleSystem _chestPS;

    [Header("Additional Chest Drop Settings")]
    public Vector2Int MinMaxDroppable;
    [Range(1.0f, 10.0f)] public float DropFrequencyModifier = 1.0f;

    private void Start()
    {
        _chestSpriteRenderer = GetComponent<SpriteRenderer>();
        _chestPS = GetComponentInChildren<ParticleSystem>();
    }

    public override void Interact()
    {
        InteractWithChest();
    }

    public void InteractWithChest()
    {
        _chestSpriteRenderer.sprite = ChestOpen;
        AudioManager.Instance.SFXAudioSource.PlayOneShot(OpenChestAudioClip);
        GameManager.Instance.DropTreasureController.SetMinMaxDroppable(MinMaxDroppable);
        GameManager.Instance.DropTreasureController.SetDropFrequencyModifier(DropFrequencyModifier);
        GameManager.Instance.DropTreasureController.DropItems(transform);

        if (_chestPS != null && _chestPS.IsAlive())
        {
            _chestPS.Stop();
        }
    }

    private void OnMouseOver()
    {
        if (gameObject != null)
        {
            GameManager.Instance.ShowItemNamePanel("Chest", transform);
        }
    }

    private void OnMouseExit()
    {
        GameManager.Instance.HideItemNamePanel();
    }
}
