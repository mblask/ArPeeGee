using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class MeeleTooltip : MonoBehaviour
{
    public Text MeeleName;
    public Text MeeleStats;

    private StringBuilder sb = new StringBuilder();

    private void Start()
    {
        HideMeeleTooltip();
    }

    public void ShowMeeleTooltip()
    {
        MeeleName.text = "Meele";

        sb.Length = 0;

        AddStat("Damage", PlayerStats.Instance.TotalDamage.GetValue());

        MeeleStats.text = sb.ToString();

        gameObject.SetActive(true);
    }

    public void HideMeeleTooltip()
    {
        gameObject.SetActive(false);
    }

    private void AddStat(string statName, float statValue)
    {
        if (sb.Length > 0)
            sb.AppendLine();

        sb.Append(statName);
        sb.Append(": ");

        sb.Append(statValue);
    }
}
