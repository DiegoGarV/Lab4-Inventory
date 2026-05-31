using UnityEngine;

public class MasterKeyLogic : StoreItemLogicBase
{
    public override void ApplyLevelStartEffect()
    {
        Debug.Log("MasterKeyLogic: llave maestra disponible para usar.");
    }

    public override bool Use(RaycastHit hit)
    {
        Debug.Log("MasterKeyLogic: llave maestra usada.");
        return true;
    }
}
