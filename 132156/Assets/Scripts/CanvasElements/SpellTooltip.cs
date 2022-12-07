using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class SpellTooltip : MonoBehaviour
{
    public Text SpellName;
    public Text SpellStats;

    private StringBuilder sb = new StringBuilder();

    private void Start()
    {
        HideSpellTooltip();
    }

    public void ShowSpellTooltip(Spell spell)
    {
        SpellName.text = spell.SpellName;

        sb.Length = 0;

        AddStat("Cost", spell.BaseSpellCost);
        AddStat("Damage", spell.BaseSpellDamage);

        SpellStats.text = sb.ToString();

        gameObject.SetActive(true);
    }

    public void HideSpellTooltip()
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

            sb.Append(statValue);
        }
    }
}
