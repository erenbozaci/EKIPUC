using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OlivePickable : Pickable
{
    public override void OnPick()
    {
        // Burada zeytin alındığında yapılacak işlemler
        Debug.Log("Zeytin alındı!");
        
        // Örneğin, oyuncunun envanterine zeytin ekleyebilirsiniz
        // PlayerInventory.Instance.AddItem(this);
        
        // Veya zeytin ile ilgili başka bir işlem yapabilirsiniz
    }
}
