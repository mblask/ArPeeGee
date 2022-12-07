using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemNamePanel : MonoBehaviour
{
    private static ItemNamePanel _instance;
    public static ItemNamePanel Instance
    {
        get
        {
            return _instance;
        }
    }

    private Text _itemNameText;

    private void Awake()
    {
        _instance = this;

        _itemNameText = transform.Find("ItemName").GetComponent<Text>();
    }

    private void Start()
    {
        if (InventoryEquipmentUI.Instance != null)
            InventoryEquipmentUI.Instance.OnInventoryUIClose += HideItemNamePanel;

        ShowItemNamePanel(false);
    }

    public void SetupPanel(Vector3 position, string itemName)
    {
        SetPanelPosition(position);
        SetItemNamePanelText(itemName);
        ShowItemNamePanel(true);
    }

    public void SetItemNamePanelText(string itemName)
    {
        _itemNameText.text = "";
        _itemNameText.text = itemName;
    }

    public void SetPanelPosition(Vector2 position)
    {
        transform.position = position;
    }

    public void ShowItemNamePanel(bool value)
    {
        gameObject.SetActive(value);
    }

    public void HideItemNamePanel()
    {
        gameObject.SetActive(false);
    }
}
