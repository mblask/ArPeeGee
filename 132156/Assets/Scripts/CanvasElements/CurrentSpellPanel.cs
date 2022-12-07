using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CurrentSpellPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Read-only")]
    [SerializeField] private SpellTooltip _tooltip;

    private Spell _currentSpell = null;

    public void SetCurrentSpell(Spell spell)
    {
        if (spell != null)
            _currentSpell = spell;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (_currentSpell != null)
            _tooltip.ShowSpellTooltip(_currentSpell);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        _tooltip.HideSpellTooltip();
    }
}
