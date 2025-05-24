using System;
using Character;
using UnityEngine;

public abstract class Equipable : MonoBehaviour
{
    public string itemName;
    public string description;
    public Sprite icon;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                Equip(player);
            }
        }
    }

    void onCollisionEnter(Collision collision)
    {
        //A
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
            }
        }
    }

    public abstract void Equip(PlayerController character);
    public abstract void Unequip(PlayerController character);
}