using UnityEngine;

public class CatalogItem : StoreItemBase
{
    protected override void ApplyEffect()
    {
        // No se usa pero si no lo pongo me da error XD
    }
    
    public override void ApplyLevelStartEffect()
    {
        Debug.Log("Revista activa: mostrar precios de los objetos.");
        // TODO: Implementar lógica para mostrar precios de los objetos que se pueden robar en el WorldStateManager.
    }
}