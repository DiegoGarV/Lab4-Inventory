using UnityEngine;

public class HouseDoorTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerMask) == 0)
            return;

        if (WorldStateManager.Instance == null)
            return;

        WorldStateManager.Instance.TogglePlayerInsideHouse();

        Debug.Log("HouseDoorTrigger: jugador cambió estado interior/exterior. Ahora dentro = " +
                  WorldStateManager.Instance.IsPlayerInsideHouse);
    }
}