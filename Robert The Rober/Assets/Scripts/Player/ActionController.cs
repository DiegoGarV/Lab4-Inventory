using UnityEngine;
using UnityEngine.InputSystem;

public class ActionController : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactMask = ~0;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;

        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnPausePressed += HandlePausePressed;
            EventManager.Instance.OnInteractPressed += HandleInteractPressed;
            EventManager.Instance.OnUseItemPressed += HandleUseItemPressed;
            EventManager.Instance.OnPurchasedInventoryPressed += HandlePurchasedInventoryPressed;
            EventManager.Instance.OnStolenInventoryPressed += HandleStolenInventoryPressed;
        }
        else
        {
            Debug.LogWarning("ActionController: EventManager.Instance es null en Start.");
        }
    }

    private void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnPausePressed -= HandlePausePressed;
            EventManager.Instance.OnInteractPressed -= HandleInteractPressed;
            EventManager.Instance.OnUseItemPressed -= HandleUseItemPressed;
            EventManager.Instance.OnPurchasedInventoryPressed -= HandlePurchasedInventoryPressed;
            EventManager.Instance.OnStolenInventoryPressed -= HandleStolenInventoryPressed;
        }
    }

    private void HandlePausePressed()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.TogglePauseMenu();
        }
    }

    private void HandlePurchasedInventoryPressed()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.TogglePurchasedItemsPanel();
        }
    }

    private void HandleStolenInventoryPressed()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleStolenItemsPanel();
        }
    }

    private void HandleInteractPressed()
    {
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null) return;
        }

        if (Physics.Raycast(
            cam.transform.position,
            cam.transform.forward,
            out RaycastHit hit,
            interactDistance,
            interactMask,
            QueryTriggerInteraction.Collide))
        {
            Pickup pickup = hit.collider.GetComponentInParent<Pickup>();

            if (pickup != null)
            {
                if (MoneyAndObjectsController.Instance != null &&
                    MoneyAndObjectsController.Instance.CanCollect(pickup))
                {
                    pickup.Collect();
                }
                else
                {
                    Debug.Log("La bolsa está llena. Solo puedes recoger billetes.");
                }

                return;
            }

            CarController car = hit.collider.GetComponentInParent<CarController>();

            if (car != null)
            {
                car.Interact();
                return;
            }

            StoreDoorController door = hit.collider.GetComponentInParent<StoreDoorController>();

            if (door != null)
            {
                door.Interact();
                return;
            }

            StoreItemBase buyableItem = hit.collider.GetComponentInParent<StoreItemBase>();

            if (buyableItem != null)
            {
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowBuyItemPrompt(buyableItem);
                }

                return;
            }
        }
    }

    private void HandleUseItemPressed()
    {
        // TODO: Implementar lógica para usar un objeto del inventario
        Debug.Log("UseItem presionado.");
    }
}