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

            DoorController heistDoor = hit.collider.GetComponentInParent<DoorController>();

            if (heistDoor != null)
            {
                if (heistDoor.CanOpenNormally())
                {
                    heistDoor.Interact();
                }
                else
                {
                    Debug.Log("La puerta está cerrada. Necesitas una herramienta para abrirla.");
                }

                return;
            }
        }
    }

    private void HandleUseItemPressed()
    {
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null) return;
        }

        if (!Physics.Raycast(
            cam.transform.position,
            cam.transform.forward,
            out RaycastHit hit,
            interactDistance,
            interactMask,
            QueryTriggerInteraction.Collide))
        {
            return;
        }

        DoorController door = hit.collider.GetComponentInParent<DoorController>();

        if (door == null)
        {
            Debug.Log("No estás apuntando a una puerta.");
            return;
        }

        if (door.CanOpenNormally())
        {
            door.Interact();
            return;
        }

        if (PlayerProgressManager.Instance == null)
            return;

        int lockpickCount = PlayerProgressManager.Instance.GetItemQuantity(LockpickItem.LockpickItemId);

        if (lockpickCount <= 0)
        {
            Debug.Log("No tienes ganzúas.");
            return;
        }

        // Consumimos una ganzúa por intento
        bool consumed = PlayerProgressManager.Instance.ConsumeItem(LockpickItem.LockpickItemId);

        if (!consumed)
        {
            Debug.Log("No se pudo consumir una ganzúa.");
            return;
        }

        bool success = LockpickItem.TryUseOnDoor(door);

        if (!success)
        {
            Debug.Log("La ganzúa se rompió o no logró abrir la puerta.");
        }
    }
}