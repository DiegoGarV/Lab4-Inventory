using UnityEngine;

public class MeatItem : StoreItemBase
{
    protected override void ApplyEffect()
    {
        Debug.Log("Carne usada: distraer al perro.");

        // TODO: lógica para usarla cerca de un perro
    }

    public override void ApplyLevelStartEffect()
    {
        Debug.Log("Carne activa: distraer al perro.");
        // TODO: lógica para guardar que ya se tiene en el ProgressManager
    }

    protected override void OnConsumedCompletely()
    {
        Debug.Log("La carne se agotó.");
    }
}