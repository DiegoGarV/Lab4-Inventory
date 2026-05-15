using UnityEngine;

public class WireCutterItem : StoreItemBase
{
    protected override void ApplyEffect()
    {
        Debug.Log("Cortacables usado: intentar desactivar energía/cámaras.");

        // TODO: lógica para desactivar energía al estar cerca de una caja de fusibles
    }

    public override void ApplyLevelStartEffect()
    {
        // TODO: guardar en el ProgressManager que y se tiene.
    }
}