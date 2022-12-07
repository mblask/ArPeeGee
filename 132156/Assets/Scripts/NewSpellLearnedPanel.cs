using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewSpellLearnedPanel : MonoBehaviour
{
    public GameObject NotificationPanel;
    public float HideAfterTime;

    private const string NewSpellLearnedString = "You've learned a new spell. Switch the spells using W.";
    private const string SpellAlreadyKnownString = "You've already learned that spell!";
    private const string InventoryFullString = "Inventory is full!";
    private const string NoMorePotionsString = "You have no potions available.";

    private void Start()
    {
        HidePanel();

        PlayerController.Instance.onNewSpellLearned += SpellLearningNotification;
        Inventory.Instance.onInventoryFull += InventoryFullNotification;
        Inventory.Instance.onNoPotions += NoPotionsNotification;
    }

    public void SpellLearningNotification(bool value)
    {
        Text panelText = NotificationPanel.GetComponentInChildren<Text>();

        if (value)
            panelText.text = NewSpellLearnedString;
        else
            panelText.text = SpellAlreadyKnownString;

        NotificationPanel.SetActive(true);

        Invoke("HidePanel", HideAfterTime);
    }

    public void InventoryFullNotification()
    {
        Text panelText = NotificationPanel.GetComponentInChildren<Text>();

        panelText.text = InventoryFullString;

        NotificationPanel.SetActive(true);

        Invoke("HidePanel", HideAfterTime);
    }

    public void NoPotionsNotification()
    {
        Text panelText = NotificationPanel.GetComponentInChildren<Text>();

        panelText.text = NoMorePotionsString;

        NotificationPanel.SetActive(true);

        Invoke("HidePanel", HideAfterTime);
    }

    public void HidePanel()
    {
        NotificationPanel.SetActive(false);
    }
}
