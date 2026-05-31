using UnityEngine;

public class BiggerBagLogic : StoreItemLogicBase
{
    [SerializeField] private float newSackCapacity = 300f;

    public override void ApplyLevelStartEffect()
    {
        if (MoneyAndObjectsController.Instance != null)
        {
            MoneyAndObjectsController.Instance.SetMaxSackLoad(newSackCapacity);
            Debug.Log($"BiggerBagLogic: capacidad del saco cambiada a {newSackCapacity}");
        }
    }
}