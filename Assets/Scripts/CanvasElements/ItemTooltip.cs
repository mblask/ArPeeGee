using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class ItemTooltip : MonoBehaviour
{
    private static ItemTooltip _instance;
    public static ItemTooltip Instance
    {
        get
        {
            return _instance;
        }
    }

    public Text ItemName;
    public Text ItemStats;

    private StringBuilder sb = new StringBuilder();

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        if (InventoryEquipmentUI.Instance != null)
            InventoryEquipmentUI.Instance.OnInventoryUIClose += HideItemTooltip;

        HideItemTooltip();
    }

    public void ShowItemTooltip(Item item)
    {
        ItemStats.gameObject.SetActive(false);

        ItemName.text = item.ItemName;

        if (item is Equipment)
        {
            ItemStats.gameObject.SetActive(true);

            Equipment equipmentItem = item as Equipment;

            sb.Length = 0;

            AddStat("Armor", equipmentItem.ArmorModifier);
            AddStat("Damage", equipmentItem.DamageModifier);
            AddStat("Attack Rate", equipmentItem.AttackRateModifier);
            AddStat("Health", equipmentItem.HealthModifier);
            AddStat("Magic", equipmentItem.MagicModifier);
            AddStat("Movement Speed", equipmentItem.MovementSpeedModifier);
            AddStat("Health Regen", equipmentItem.HealthRegenModifier);
            AddStat("Magic Regen", equipmentItem.MagicRegenModifier);

            ItemStats.text = sb.ToString();
        }

        gameObject.SetActive(true);
    }

    public void HideItemTooltip()
    {
        gameObject.SetActive(false);
    }

    private void AddStat(string statName, float statValue)
    {
        if (statValue != 0.0f)
        {
            if (sb.Length > 0)
                sb.AppendLine();

            sb.Append(statName);
            sb.Append(": ");

            if (statValue > 0)
                sb.Append("+");

            sb.Append(statValue);
        }
    }
}
