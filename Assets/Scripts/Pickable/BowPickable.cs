
using Character;
using UnityEngine;

public class BowPickable : WeaponPickable
{
    public override void OnPick()
    {
        GameObject weaponPrefab = new GameObject();
        weaponPrefab.AddComponent<Bow>().Initialize(
            this.weaponName,
            this.damage,
            this.attackCooldown,
            this.attackRange
        );
        weaponPrefab.name = base.weaponName;
        WeaponManager.Instance.EquipWeapon(weaponPrefab);
    }
}
