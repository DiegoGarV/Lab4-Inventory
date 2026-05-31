using UnityEngine;

public class MeatLogic : StoreItemLogicBase
{
    public override void ApplyLevelStartEffect()
    {
        Debug.Log("MeatLogic: carne disponible para usar.");
    }

    public override bool Use(RaycastHit hit)
    {
        Debug.Log("MeatLogic: carne usada.");
        return true;
    }
}