using UnityEngine;

public class MasterKeyItem : StoreItemBase
{
    protected override void ApplyEffect()
    {
        // TODO: lógica para que se active cuando se usa con una puerta
    }

    public override void ApplyLevelStartEffect()
    {
        Debug.Log("Llave maestra activa: puede abrir todas las puertas.");
        // TODO: controlador en el ProgressManager para el guardado
    }
}