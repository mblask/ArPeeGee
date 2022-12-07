using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    public Transform AttackPoint;
    public LayerMask EnemyLayerMask;

    [Header("Read-only")]

    private Equipment _weaponItem;
    private Animator _animator;

    public Equipment WeaponItem => _weaponItem;

    private void Start()
    {
        InventoryEquipmentUI.Instance.OnWeaponChanged += SetupWeapon;

        if (EquipmentManager.Instance != null)
        {
            EquipmentManager.Instance.onEquipmentChanged += SetupWeaponSlot;
        }
        _animator = GetComponent<Animator>();
    }

    public void SetupWeaponSlot(Equipment newItem, Equipment oldItem)
    {
        if (newItem != null)
        {
            if (newItem.EquipmentSlot == EquipmentSlot.Weapon)
            {
                if (oldItem == null)
                {
                    //Equip into empty
                    SetupWeapon(newItem);
                }

                if (oldItem != null)
                {
                    //Replace existing with new
                    RemoveAllWeapons();
                    SetupWeapon(newItem);
                }
            }
            else
            {
                //Item(s) not adequate
                return;
            }
        }

        if (newItem == null)
        {
            if (oldItem != null)
            {
                if (oldItem.EquipmentSlot == EquipmentSlot.Weapon)
                {
                    //Unequip weapon
                    RemoveAllWeapons();
                }
                else
                {
                    //Item(s) not adequate
                    return;
                }
            }
        }
    }

    public void SetupWeapon(Equipment weapon)
    {
        if (weapon == null)
            return;

        _weaponItem = weapon;

        GameObject weaponItem = Instantiate(weapon.ItemPrefab, transform.position, transform.rotation);
        /**/
        weaponItem.GetComponent<ItemPickup>().Item = weapon;
        weaponItem.GetComponent<SpriteRenderer>().sprite = weapon.ItemImage;
        /**/
        weaponItem.transform.SetParent(transform);
        weaponItem.GetComponent<ItemPickup>().enabled = false;
        weaponItem.layer = LayerMask.NameToLayer("EquippedItems");
        weaponItem.GetComponent<SpriteRenderer>().sortingOrder = 7;

        float playerLocalXScale = GetComponentsInParent<Transform>()[GetComponentsInParent<Transform>().Length - 1].localScale.x;

        if (playerLocalXScale < 0.0f)
        {
            Vector3 weaponSlotLocalScale = transform.localScale;
            weaponSlotLocalScale.x *= -1;
            transform.localScale = weaponSlotLocalScale;
        }
    }

    public void RemoveWeapon(Equipment weapon)
    {
        if (weapon == null)
            return;

        int childrenCount = transform.childCount;
        for (int i = 0; i < childrenCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.GetComponent<ItemPickup>().Item.ItemName == weapon.ItemName) //Can I write something better?????
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void RemoveAllWeapons()
    {
        int childrenCount = transform.childCount;

        for (int i = 0; i < childrenCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void WeaponSlotAttack()
    {
        if (_weaponItem == null)
        {
            Debug.LogWarning("There is no weapon");
            return;
        }
            
        _animator.SetTrigger("Attack");
        AudioManager.Instance.SFXAudioSource.PlayOneShot(AudioManager.Instance.WeaponSwingAudioClip);

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(AttackPoint.position, _weaponItem.AttackRange, EnemyLayerMask);
        if (hitColliders.Length > 0)
        {
            if (IsHitting(hitColliders[0].GetComponent<EnemyStats>().TotalEnemyArmor.GetValue()))
            {
                AudioManager.Instance.SFXAudioSource.PlayOneShot(AudioManager.Instance.WeaponHitAudioClip);
                hitColliders[0].GetComponent<EnemyStats>().DamageManager(PlayerStats.Instance.TotalDamage.GetValue());
            }
        }
    }

    public bool IsHitting(float armor)
    {
        float attackArmorRatio = -PlayerStats.Instance.TotalDamage.GetValue() / armor;
        float hitChance = 1.0f - 0.6f * Mathf.Pow(10, attackArmorRatio);

        float missChance = Random.Range(0, 100) / 100.0f;

        if (hitChance > missChance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsEquipped()
    {
        return _weaponItem != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (_weaponItem == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(AttackPoint.position, _weaponItem.AttackRange);
    }
}
