using UnityEngine;

public class CatalogLogic : StoreItemLogicBase
{
    public override void ApplyLevelStartEffect()
    {
        if (PlayerProgressManager.Instance != null)
        {
            PlayerProgressManager.Instance.SetCanSeeItemPrices(true);
            Debug.Log("CatalogLogic: ahora se puede ver el valor de los objetos.");
        }
    }
}
