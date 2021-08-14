using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "ArPeeGee/Items/Consumable", order = 3)]
public class Consumable : Item
{
    public float LifeRegen;
    public float MagicRegen;

    public AudioClip ConsumationAudioClip;

    public override void Use()
    {
        AudioManager.Instance.SFXAudioSource.PlayOneShot(ConsumationAudioClip);
        PlayerStats.Instance.UpdateHealth(LifeRegen);
        PlayerStats.Instance.UpdateMagic(MagicRegen);
        RemoveFromInventory();
    }
}