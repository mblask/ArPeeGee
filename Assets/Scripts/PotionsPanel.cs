using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionsPanel : MonoBehaviour
{
    public Image HealthPotionImage;
    public Text HealthPotionNumber;
    public Image MagicPotionImage;
    public Text MagicPotionNumber;

    private Inventory _inventory;
    private int _healthPotionNumber = 0;
    private int _magicPotionNumber = 0;

    private Color activeColor = new Color(255.0f, 255.0f, 255.0f, 255.0f);
    private Color inactiveColor = new Color(255.0f, 255.0f, 255.0f, 127.0f);

    private void Start()
    {
        _inventory = Inventory.Instance;

        UpdatePotionsPanel();

        _inventory.onInventoryItemsChanged += UpdatePotionsPanel;
    }

    public void UpdatePotionsPanel()
    {
        _healthPotionNumber = CheckForPotions(PotionType.Health);
        _magicPotionNumber = CheckForPotions(PotionType.Magic);

        if (_healthPotionNumber != 0)
        {
            HealthPotionNumber.text = _healthPotionNumber.ToString();
            HealthPotionImage.color = activeColor;
        }
        else
        {
            HealthPotionNumber.text = "0";
            HealthPotionImage.color = inactiveColor;
        }

        if (_magicPotionNumber != 0)
        {
            MagicPotionNumber.text = _magicPotionNumber.ToString();
            MagicPotionImage.color = activeColor;
        }
        else
        {
            MagicPotionNumber.text = "0";
            MagicPotionImage.color = inactiveColor;
        }
    }

    private int CheckForPotions(PotionType type)
    {
        int potionCount = 0;

        foreach (Item item in _inventory._items)
        {
            if (item is Consumable)
            {
                if (item.ItemName.Contains(type.ToString()))
                    potionCount++;
            }
        }

        return potionCount;
    }
}
