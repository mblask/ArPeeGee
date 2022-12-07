using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CurrentMeelePanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Read-only")]
    [SerializeField] private MeeleTooltip _tooltip;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        _tooltip.ShowMeeleTooltip();
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        _tooltip.HideMeeleTooltip();
    }
}
