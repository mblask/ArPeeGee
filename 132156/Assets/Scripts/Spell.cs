using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellType
{
    Projectile,
    AreaOfEffect
}

public enum SpellElement
{
    Fire,
    Ice
}

public class Spell : MonoBehaviour
{
    [Header("Base Spell Descriptors")]
    public string SpellName;
    public Sprite SpellIcon;
    public SpellType SpellType;
    public SpellElement SpellElement;

    [Header("Base Spell Characteristics")]
    public float BaseSpellCost;
    public float BaseSpellDamage;
}
