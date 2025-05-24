using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OlivePickable : Pickable
{
    public override void OnPick()
    {
        Debug.Log("Zeytin alındı!");
        PlayerPotionManager.Instance.UsePotion(new Potion("Olive", 5f, 10));
    }
}