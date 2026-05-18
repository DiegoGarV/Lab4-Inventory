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
    public InputDeviceType CurrentDeviceType { get; private set; } = InputDeviceType.Unknown;

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
        UpdateCurrentDeviceType();
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

    private void UpdateCurrentDeviceType()
    {
        if (Gamepad.current != null)
        {
            if (Gamepad.current.leftStick.ReadValue().sqrMagnitude > 0.001f ||
                Gamepad.current.rightStick.ReadValue().sqrMagnitude > 0.001f ||
                Gamepad.current.buttonSouth.wasPressedThisFrame ||
                Gamepad.current.buttonEast.wasPressedThisFrame ||
                Gamepad.current.startButton.wasPressedThisFrame ||
                Gamepad.current.leftShoulder.wasPressedThisFrame ||
                Gamepad.current.rightShoulder.wasPressedThisFrame)
            {
                CurrentDeviceType = InputDeviceType.Gamepad;
                return;
            }
        }

        if (Mouse.current != null)
        {
            if (Mouse.current.delta.ReadValue().sqrMagnitude > 0.001f ||
                Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.rightButton.wasPressedThisFrame)
            {
                CurrentDeviceType = InputDeviceType.KeyboardAndMouse;
                return;
            }
        }

        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            CurrentDeviceType = InputDeviceType.KeyboardAndMouse;
        }
    }
}