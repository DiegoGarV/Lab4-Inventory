using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    private GameInputActions inputActions;

    public event Action OnMoneyCollected;

    public event Action OnInteractPressed;
    public event Action OnUseItemPressed;
    public event Action OnPurchasedInventoryPressed;
    public event Action OnStolenInventoryPressed;
    public event Action OnPausePressed;
    public event Action OnJumpPressed;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        inputActions = new GameInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.GameMap.Interact.performed += HandleInteractPerformed;
        inputActions.GameMap.UseItem.performed += HandleUseItemPerformed;
        inputActions.GameMap.PurchasedInventory.performed += HandlePurchasedInventoryPerformed;
        inputActions.GameMap.StolenInventory.performed += HandleStolenInventoryPerformed;
        inputActions.GameMap.PauseMenu.performed += HandlePausePerformed;
        inputActions.GameMap.Jump.performed += HandleJumpPerformed;
    }

    private void OnDisable()
    {
        inputActions.GameMap.Interact.performed -= HandleInteractPerformed;
        inputActions.GameMap.UseItem.performed -= HandleUseItemPerformed;
        inputActions.GameMap.PurchasedInventory.performed -= HandlePurchasedInventoryPerformed;
        inputActions.GameMap.StolenInventory.performed -= HandleStolenInventoryPerformed;
        inputActions.GameMap.PauseMenu.performed -= HandlePausePerformed;
        inputActions.GameMap.Jump.performed -= HandleJumpPerformed;

        inputActions.Disable();
    }

    private void Update()
    {
        MoveInput = inputActions.GameMap.Move.ReadValue<Vector2>();
        LookInput = inputActions.GameMap.Look.ReadValue<Vector2>();
    }

    private void HandleInteractPerformed(InputAction.CallbackContext context)
    {
        OnInteractPressed?.Invoke();
    }

    private void HandleUseItemPerformed(InputAction.CallbackContext context)
    {
        OnUseItemPressed?.Invoke();
    }

    private void HandlePurchasedInventoryPerformed(InputAction.CallbackContext context)
    {
        OnPurchasedInventoryPressed?.Invoke();
    }

    private void HandleStolenInventoryPerformed(InputAction.CallbackContext context)
    {
        OnStolenInventoryPressed?.Invoke();
    }

    private void HandlePausePerformed(InputAction.CallbackContext context)
    {
        OnPausePressed?.Invoke();
    }

    private void HandleJumpPerformed(InputAction.CallbackContext context)
    {
        OnJumpPressed?.Invoke();
    }

    public void MoneyCollected()
    {
        OnMoneyCollected?.Invoke();
    }
}