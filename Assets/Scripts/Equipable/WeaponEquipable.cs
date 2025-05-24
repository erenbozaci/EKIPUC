using Character;
using UnityEngine;

public enum WeaponType
{
    BOW,
    SWORDSHIELD
}
public abstract class WeaponEquipable : Equipable
{
    [Header("Weapon Settings")]
    public float attackRange = 2f;
    public int damage = 10;
    public float attackCooldown = 1f;

    public abstract void Attack(Vector3 targetPosition);

    public override void Equip(PlayerController character) // OnEquip yerine Equip olarak değiştirildi
    {
        WeaponManager.Instance.EquipWeapon(this);
    }

    public override void Unequip(PlayerController character) // OnUnequip yerine Unequip olarak değiştirildi
    {
        WeaponManager.Instance.DestroyWeapon();
    }
}
