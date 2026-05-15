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
    }

    private void Update()
    {
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null) return;
        }

        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.TogglePauseMenu();
            }

            return;
        }

        bool interactPressed = false;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            interactPressed = true;

        if (!interactPressed) return;

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
}