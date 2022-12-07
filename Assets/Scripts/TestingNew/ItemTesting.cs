using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTesting : MonoBehaviour
{
    [SerializeField] private ItemStruct Item;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _spriteRenderer.sprite = Item.Item.ItemImage;
    }
}
