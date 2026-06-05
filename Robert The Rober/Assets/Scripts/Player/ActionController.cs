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

    private void Update()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsBlockingGameplayInput)
        {
            UIManager.Instance.HideSackableHint();
            return;
        }

        UpdateSackableHint();
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
        if (UIManager.Instance != null && UIManager.Instance.IsBlockingGameplayInput)
            return;

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
                    EntityID entityId = pickup.GetComponent<EntityID>();

                    if (entityId == null)
                        entityId = pickup.GetComponentInParent<EntityID>();

                    if (entityId != null && WorldStateManager.Instance != null)
                    {
                        WorldStateManager.Instance.RegisterStolenThing(entityId.ID);
                    }

                    pickup.Collect();
                }
                else
                {
                    Debug.Log("Este objeto ya no cabe en la bolsa.");
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

            StorePurchaseItem buyableItem = hit.collider.GetComponentInParent<StorePurchaseItem>();

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
                heistDoor.Interact();
                return;
            }
        }
    }

    private void HandleUseItemPressed()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsBlockingGameplayInput)
            return;

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

        if (PlayerProgressManager.Instance == null)
            return;

        if (StoreItemLogicManager.Instance == null)
        {
            Debug.LogWarning("StoreItemLogicManager.Instance es null.");
            return;
        }

        // 1. Intentar primero Master Key
        MasterKeyLogic masterKeyLogic = StoreItemLogicManager.Instance.GetLogic<MasterKeyLogic>();

        if (masterKeyLogic != null &&
            PlayerProgressManager.Instance.HasItem(masterKeyLogic.ItemId))
        {
            bool used = masterKeyLogic.Use(hit);
            if (used)
                return;
        }

        // 2. Intentar luego Lockpick
        LockpickLogic lockpickLogic = StoreItemLogicManager.Instance.GetLogic<LockpickLogic>();

        if (lockpickLogic != null &&
            PlayerProgressManager.Instance.GetItemQuantity(lockpickLogic.ItemId) > 0)
        {
            bool used = lockpickLogic.Use(hit);
            if (used)
                return;
        }

        // 3. Intentar los demás items comprados
        foreach (PurchasedStoreItemData purchasedItem in PlayerProgressManager.Instance.PurchasedItems)
        {
            if (purchasedItem == null || !purchasedItem.wasPurchased)
                continue;

            if (masterKeyLogic != null && purchasedItem.itemId == masterKeyLogic.ItemId)
                continue;

            if (lockpickLogic != null && purchasedItem.itemId == lockpickLogic.ItemId)
                continue;

            if (purchasedItem.quantity <= 0)
                continue;

            StoreItemLogicBase logic = StoreItemLogicManager.Instance.GetLogicById(purchasedItem.itemId);

            if (logic == null)
                continue;

            bool wasUsed = logic.Use(hit);

            if (wasUsed)
                return;
        }

        bool interactionSupportsSomeItem = false;

        // Revisar si el hit era válido para algún item lógico, aunque no lo tengas
        foreach (StoreItemLogicBase logic in StoreItemLogicManager.Instance.AllLogics)
        {
            if (logic == null)
                continue;

            if (logic.CanUseOn(hit))
            {
                interactionSupportsSomeItem = true;
                break;
            }
        }

        if (interactionSupportsSomeItem)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMissingRequiredItem();
            }

            Debug.Log("Te falta el objeto necesario para interactuar aquí.");
            return;
        }

        Debug.Log("Ningún item comprado pudo usarse aquí.");
    }

    private void UpdateSackableHint()
    {
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null) return;
        }

        if (UIManager.Instance == null)
            return;

        if (Physics.Raycast(
            cam.transform.position,
            cam.transform.forward,
            out RaycastHit hit,
            interactDistance,
            interactMask,
            QueryTriggerInteraction.Collide))
        {
            if (hit.collider.CompareTag("Sackable"))
            {
                Pickup pickup = hit.collider.GetComponentInParent<Pickup>();

                if (pickup != null)
                {
                    bool showPrice = PlayerProgressManager.Instance != null &&
                                    PlayerProgressManager.Instance.CanSeeItemPrices;

                    UIManager.Instance.ShowSackableHint(
                        pickup.SackValue,
                        pickup.MonetaryValue,
                        showPrice
                    );

                    return;
                }
            }
        }

        UIManager.Instance.HideSackableHint();
    }
}