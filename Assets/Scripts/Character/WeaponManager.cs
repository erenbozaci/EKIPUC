using UnityEngine;

namespace Character
{
    public class WeaponManager : MonoBehaviour
    {
        [Header("Weapon Settings")]
        [SerializeField] private WeaponAbstract currentWeapon;
        [SerializeField] private Transform weaponHolder;

        public WeaponAbstract CurrentWeapon => currentWeapon;
        
        public static WeaponManager Instance { get; private set; }

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void EquipWeapon(WeaponEquipable weaponEquipable)
        {
            if (currentWeapon != null)
            {
                Destroy(currentWeapon.gameObject);
            }
            currentWeapon = Instantiate(weaponEquipable.getWeapon(), weaponHolder.position, weaponHolder.rotation, weaponHolder);
        }
        
        public void DestroyWeapon()
        {
            if (currentWeapon != null)
            {
                Destroy(currentWeapon.gameObject);
                currentWeapon = null;
            }
        }

        public void Attack(Vector3 targetPosition)
        {
            if (currentWeapon != null)
            {
                currentWeapon.Attack(targetPosition);
            }
        }
    }
}