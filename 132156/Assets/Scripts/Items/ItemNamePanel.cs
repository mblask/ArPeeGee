using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemNamePanel : MonoBehaviour
{
    public Text ItemNameText;

    private void Start()
    {
        ShowItemNamePanel(false);
    }

    public void SetItemNamePanelText(string itemName)
    {
        ClearItemNamePanelText();
        ItemNameText.text = itemName;
    }

    public void SetItemNameTextColor(Color newColor)
    {
        ItemNameText.color = newColor;
    }

    public void ClearItemNamePanelText()
    {
        ItemNameText.text = "";
    }

    public void ShowItemNamePanel(bool value)
    {
        gameObject.SetActive(value);
    }
}
