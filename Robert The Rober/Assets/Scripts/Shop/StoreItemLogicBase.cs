using UnityEngine;

public abstract class StoreItemLogicBase : MonoBehaviour
{
    [SerializeField] private string itemId;

    public string ItemId => itemId;

    public virtual void ApplyLevelStartEffect()
    {
    }

    public virtual bool Use(RaycastHit hit)
    {
        return false;
    }

    public virtual bool CanUseOn(RaycastHit hit)
    {
        return false;
    }
}