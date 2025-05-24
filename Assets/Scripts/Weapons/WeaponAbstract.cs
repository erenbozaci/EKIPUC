using UnityEngine;

public abstract class WeaponAbstract : MonoBehaviour
{
    public string weaponName;
    public int damage;
    public float attackSpeed;
    public float range;
    
    public abstract void Attack(Vector3 targetPosition);
    
    public virtual void DisplayInfo()
    {
        Debug.Log($"Weapon: {weaponName}, Damage: {damage}, Attack Speed: {attackSpeed}, Range: {range}");
    }

    public virtual void Initialize(string name, int dmg, float speed, float rng)
    {
        weaponName = name;
        damage = dmg;
        attackSpeed = speed;
        range = rng;
    }
}