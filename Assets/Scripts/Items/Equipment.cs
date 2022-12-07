using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "ArPeeGee/Items/Equipment", order = 2)]
public class Equipment : Item
{
    [Header("Equipment Stats")]
    public EquipmentSlot EquipmentSlot;
    public float ArmorModifier;
    public float DamageModifier;
    public float AttackRange;
    public float AttackRateModifier;
    public float CastingRateModifier;
    public float HealthModifier;
    public float MagicModifier;
    public float MovementSpeedModifier;
    public float HealthRegenModifier;
    public float MagicRegenModifier;

    public override void Use()
    {
        //if(EquipmentManager.Instance != null && EquipmentManager.Instance.Equip(this))
        //    RemoveFromInventory();

        if (PlayerEquipment.Instance != null)
            PlayerEquipment.Instance.EquipItem(this);
    }
}