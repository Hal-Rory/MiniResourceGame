using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[Serializable]
public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera _activeCamera;
    [SerializeField] private PlayerInput _playerInput;

    private InputAction _primaryAction;
    private InputAction _secondaryAction;
    private InputAction _exitAction;
    private InputAction _moveAction;
    private InputAction _menuAction;
    private InputAction _pointAction;
    private Vector2 _mousePosition;

    private Vector3 _lastPosition;
    public Vector2 Movement { get; private set; }
    public bool PrimaryPressed;
    public bool SecondaryPressed;
    public bool ExitPressed;
    public bool MenuPressed;
    public string ControlScheme;

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    private void Start()
    {
        _primaryAction = _playerInput.actions["Primary"];
        _secondaryAction = _playerInput.actions["Secondary"];
        _menuAction = _playerInput.actions["Menu"];
        _exitAction = _playerInput.actions["Exit"];
        _moveAction = _playerInput.actions["Move"];
        _pointAction = _playerInput.actions["Point"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
        _mousePosition = _pointAction.ReadValue<Vector2>();
        PrimaryPressed = _primaryAction.WasPressedThisFrame();
        SecondaryPressed = _secondaryAction.WasPressedThisFrame();
        ExitPressed = _exitAction.WasPressedThisFrame();
        MenuPressed = _menuAction.WasPressedThisFrame();
    }

    public Vector3 GetSelectionPosition(LayerMask layer, out Collider2D col)
    {
        Vector3 mousePos = _mousePosition;
        mousePos.z = _activeCamera.nearClipPlane;
        Ray ray = _activeCamera.ScreenPointToRay(mousePos);
        RaycastHit2D
            hit = Physics2D.Raycast(ray.origin, ray.direction, 100, layer);
        if (hit.collider)
        {
            _lastPosition = hit.point;
        }

        col = hit.collider;
        Debug.DrawLine(_activeCamera.transform.position, hit.point);
        return _lastPosition;
    }

    public Vector3 ScreenToWorld(Vector3 position)
    {
        return _activeCamera.ScreenToWorldPoint(position);
    }

    public Vector3 WorldToScreen(Vector3 position)
    {
        return _activeCamera.WorldToScreenPoint(position);
    }
}