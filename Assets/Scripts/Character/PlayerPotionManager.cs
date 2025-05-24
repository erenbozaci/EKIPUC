using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion
{
    public string potionID; // Unique identifier for the potion
    public string potionName;
    public float duration;
    public int effectStrength;

    public Potion(string name, float dur, int strength)
    {
        potionName = name;
        duration = dur;
        effectStrength = strength;
    }
    
    
}
public class PlayerPotionManager : MonoBehaviour
{
    public static PlayerPotionManager Instance;
    public List<Potion> usingPotions = new List<Potion>();
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void RemovePotion(Potion potion)
    {
        if (usingPotions.Contains(potion))
        {
            usingPotions.Remove(potion);
            Debug.Log($"Potion {potion.potionName} removed");
        }
        else
        {
            Debug.LogWarning($"Potion {potion.potionName} not found in the list");
        }
    }
    public void ClearPotions()
    {
        usingPotions.Clear();
        Debug.Log("All potions cleared");
    }
    
    public void UsePotion(Potion potion)
    {
        usingPotions.Add(potion);
        StartCoroutine(WaitForPotionDuration(potion));
    }
    
    private IEnumerator WaitForPotionDuration(Potion potion)
    {
        yield return new WaitForSeconds(potion.duration);
        RemovePotion(potion);
        Debug.Log($"Potion {potion.potionName} effect ended");
    }
}
