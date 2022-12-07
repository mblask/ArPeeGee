using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsName : MonoBehaviour
{
    public Text PlayerName;

    private void Start()
    {
        if (PlayerController.Instance.gameObject.name == ConstantContainer.PlayerWizardText)
            PlayerName.text = ConstantContainer.WizardText;
        else if (PlayerController.Instance.gameObject.name == ConstantContainer.PlayerKnightText)
            PlayerName.text = ConstantContainer.KnightText;
        else
            Debug.LogWarning("Other name");
    }
}
