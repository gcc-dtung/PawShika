using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private InputActionReference pointAction;
    [SerializeField] private InputActionReference pressAction;

    public bool IsPointerPressed;
    public Vector2 PointerPosition;
    
    private void OnEnable()
    {
        pointAction.action.performed += OnPointPerformed;
        pressAction.action.performed += OnPressStarted;
        pressAction.action.canceled += OnPressCanceled;
        pointAction.action.Enable();
    }

    private void OnDisable()
    {
        pointAction.action.performed -= OnPointPerformed;
        pressAction.action.started -= OnPressStarted;
        pressAction.action.canceled -= OnPressCanceled;
        pointAction.action.actionMap.Disable();
        IsPointerPressed = false;
    }

    private void OnPointPerformed(InputAction.CallbackContext context)
    {
        PointerPosition = context.ReadValue<Vector2>();
    }
    
    private void OnPressStarted(InputAction.CallbackContext context)
    {
        IsPointerPressed = true;
    }
    
    private void OnPressCanceled(InputAction.CallbackContext context)
    {
        IsPointerPressed = false;
    }
}
