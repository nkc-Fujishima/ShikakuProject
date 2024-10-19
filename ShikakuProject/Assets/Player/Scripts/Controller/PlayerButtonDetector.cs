using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerButtonDetector
{
    private PlayerInput _playerInput;

    private Vector3 _inputStick = new();


    internal Vector3 InputStick => _inputStick;

    internal bool IsFire {  get; private set; }

    internal UnityEvent OnButtonFireDown = new();
    internal UnityEvent OnButtonBulletSelectLeftDown = new();
    internal UnityEvent OnButtonBulletSelectRightDown = new();
    internal UnityEvent OnButtonAvoidDown = new();


    internal PlayerButtonDetector(PlayerInput playerInput)
    {
        _playerInput = playerInput;

        SetDelegate();
    }

    private void SetDelegate()
    {
        _playerInput.actions["Move"].performed += OnInputMove;
        _playerInput.actions["Fire"].performed += OnInputFire;
        _playerInput.actions["BulletSelectLeft"].performed += OnInputBulletSelectLeft;
        _playerInput.actions["BulletSelectRight"].performed += OnInputBulletSelectRight;
        _playerInput.actions["Avoid"].performed += OnInputAvoid;
    }

    private void OnInputMove(InputAction.CallbackContext context)
    {
        _inputStick = context.ReadValue<Vector3>();
    }

    private void OnInputFire(InputAction.CallbackContext context)
    {
        OnButtonFireDown?.Invoke();
    }

    private void OnInputBulletSelectLeft(InputAction.CallbackContext context)
    {
        OnButtonBulletSelectLeftDown?.Invoke();
    }

    private void OnInputBulletSelectRight(InputAction.CallbackContext context)
    {
        OnButtonBulletSelectRightDown?.Invoke();
    }

    private void OnInputAvoid(InputAction.CallbackContext context)
    {
        OnButtonAvoidDown?.Invoke();
    }
}