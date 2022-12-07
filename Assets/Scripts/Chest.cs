using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    public Sprite ChestOpen;
    public AudioClip OpenChestAudioClip;

    private Camera _camera;
    private SpriteRenderer _chestSpriteRenderer;
    private ParticleSystem _chestPS;

    private bool _chestLooted = false;

    [Header("Additional Chest Drop Settings")]
    public Vector2Int MinMaxDroppable;
    [Range(1.0f, 10.0f)] public float DropFrequencyModifier = 1.0f;

    private void Start()
    {
        _camera = Camera.main;
        _chestSpriteRenderer = GetComponent<SpriteRenderer>();
        _chestPS = GetComponentInChildren<ParticleSystem>();
    }

    public override void Interact()
    {
        if (!_chestLooted)
        {
            _chestLooted = true;

            _chestSpriteRenderer.sprite = ChestOpen;
            AudioManager.Instance.SFXAudioSource.PlayOneShot(OpenChestAudioClip);
            DropTreasure.Instance.SetMinMaxDroppable(MinMaxDroppable);
            DropTreasure.Instance.SetDropFrequencyModifier(DropFrequencyModifier);
            DropTreasure.Instance.DropItems(transform);

            if (_chestPS != null && _chestPS.IsAlive())
            {
                _chestPS.Stop();
            }
        }
    }

    private void OnMouseOver()
    {
        ItemNamePanel.Instance?.SetupPanel(_camera.WorldToScreenPoint(transform.position), "Chest");
    }

    private void OnMouseExit()
    {
        ItemNamePanel.Instance?.ShowItemNamePanel(false);
    }
}
