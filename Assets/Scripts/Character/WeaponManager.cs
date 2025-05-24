using System;
using UnityEngine;

namespace Character
{
    public class WeaponManager : MonoBehaviour
    {
        [Header("Weapon Settings")]
        [SerializeField] private GameObject currentWeapon;
        [SerializeField] private Transform weaponHolder;

        public GameObject CurrentWeapon => currentWeapon;
        
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                DestroyWeapon();
            } 
            else if (Input.GetMouseButtonDown(0))
            {
                Attack();
            }
        }

        public void EquipWeapon(GameObject weaponPrefab)
        {
            if (currentWeapon != null)
            {
                Destroy(currentWeapon.gameObject);
            }
            
            currentWeapon = Instantiate(weaponPrefab, weaponHolder.position, weaponHolder.rotation, weaponHolder);
        }
        
        public void DestroyWeapon()
        {
            if (currentWeapon != null)
            {
                Destroy(currentWeapon.gameObject);
                currentWeapon = null;
            }
        }

        public void Attack()
        {
            if (currentWeapon != null)
            {
                currentWeapon.GetComponent<WeaponAbstract>().Attack(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
    }
}