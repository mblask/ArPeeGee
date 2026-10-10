using UnityEngine;

public class ItemPickup : Interactable
{
    [SerializeField]
    private Item _item;
    private Camera _camera;

    private Inventory _inventory;
    private ItemNamePanel _itemNamePanel;
    private GameManager _gameManager;
    private AudioManager _audioManager;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _camera = Camera.main;
        _inventory = Inventory.Instance;
        _itemNamePanel = ItemNamePanel.Instance;
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _spriteRenderer = GetComponent<SpriteRenderer>();

        BringItemForward();
    }

    public Item GetItem()
    {
        return _item;
    }

    public void SetItem(Item item)
    {
        Debug.Log($"SetItem item: {item == null}");

        _item = item;

        if (_spriteRenderer == null)
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        _spriteRenderer.sprite = item.ItemImage;
    }

    public override void Interact()
    {
        base.Interact();

        PickupItem();
    }

    public void PickupItem()
    {
        Debug.Log($"PickupItem null {_item == null}");
        bool wasPickedUp = _inventory.AddItem(_item);

        if (wasPickedUp)
        {
            _itemNamePanel?.ShowItemNamePanel(false);
            _gameManager?.SetCursor(CursorType.Default);
            _audioManager?.SFXAudioSource.PlayOneShot(AudioManager.Instance.ItemPickupAudioClip);
    
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
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);
        if (_itemNamePanel == null)
            return;

        if (_item == null)
            return;

        _itemNamePanel.SetupPanel(screenPosition, _item.ItemName);
    }
    
    private void OnMouseExit()
    {
        _itemNamePanel?.ShowItemNamePanel(false);
    }
}
