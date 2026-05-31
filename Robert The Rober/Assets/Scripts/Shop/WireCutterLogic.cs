using UnityEngine;

public class WireCutterLogic : StoreItemLogicBase
{
    public override void ApplyLevelStartEffect()
    {
        Debug.Log("WireCutterLogic: corta cables disponibles para usar.");
    }

    public override bool Use(RaycastHit hit)
    {
        Debug.Log("WireCutterLogic: corta cables usados.");
        return true;
    }
}
