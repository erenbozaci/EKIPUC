using Character;
using UnityEngine;

public enum WeaponType
{
    BOW,
    SWORDSHIELD
}
public abstract class WeaponPickable : Pickable
{
    [Header("Weapon Settings")]
    public string weaponName = "Default Weapon";
    public float attackRange = 2f;
    public int damage = 10;
    public float attackCooldown = 1f;

    

    public WeaponAbstract getWeapon()
    {
        if (GetComponent<WeaponAbstract>() != null)
        {
            return GetComponent<WeaponAbstract>();
        }
        else
        {
            Debug.LogError("WeaponAbstract bileşeni bu silah üzerinde bulunamadı!");
            return null;
        }
    }

    public override void OnPick()
    {
        if (WeaponManager.Instance.CurrentWeapon != null)
        {
            return;
        }
        else
        {
            WeaponManager.Instance.EquipWeapon(gameObject);
        }
        
    }
}
