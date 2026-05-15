using UnityEngine;

public class BiggerBagItem : StoreItemBase
{
    [SerializeField] private float newSackCapacity = 300f;

    protected override void ApplyEffect()
    {
        // No se usa pero si no lo pongo me da error XD
    }

    public override void ApplyLevelStartEffect()
    {
        Debug.Log("Saco++ activo: capacidad aumentada a " + newSackCapacity);

        if (MoneyAndObjectsController.Instance != null)
        {
            // TODO: setter para la capacidad del saco
        }
    }
}