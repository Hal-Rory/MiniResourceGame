using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[Serializable]
public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera _activeCamera;
    private Vector3 _lastPosition;
    private Vector2 _mousePosition;
    public event Action PrimaryPressed;
    public event Action SecondaryPressed;
    public event Action ExitPressed;

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

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

    public void PressPrimary(InputAction.CallbackContext context)
    {
        int value = (int)context.ReadValue<float>();
        if(context.started && value == 1) PrimaryPressed?.Invoke();
    }

    public void PressSecondary(InputAction.CallbackContext context)
    {
        int value = (int)context.ReadValue<float>();
        if(context.started && value == 1) SecondaryPressed?.Invoke();
    }

    public void PressExit(InputAction.CallbackContext context)
    {
        int value = (int)context.ReadValue<float>();
        if(context.started && value == 1) ExitPressed?.Invoke();
    }

    public void MousePosition(InputAction.CallbackContext context)
    {
        _mousePosition = context.ReadValue<Vector2>();
    }
}