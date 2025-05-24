using Character;
using UnityEngine;

public abstract class Equipable : MonoBehaviour
{
    public string itemName;
    public string description;
    public Sprite icon;
    
    
    
    public abstract void Equip(PlayerController character);
    public abstract void Unequip(PlayerController character);
}