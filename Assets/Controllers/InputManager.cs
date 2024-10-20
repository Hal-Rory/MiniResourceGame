using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera _activeCamera;
    private Vector3 _lastPosition;
    [SerializeField] private LayerMask _placementLayers;
    public event Action OnSelectPrimary;
    public event Action OnSelectSecondary;
    public event Action<Vector2> OnScroll;
    public event Action OnExit;

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
            OnScroll?.Invoke(Input.mouseScrollDelta);
        if (Input.GetMouseButtonDown(0))
            OnSelectPrimary?.Invoke();
        if (Input.GetMouseButtonDown(1))
            OnSelectSecondary?.Invoke();
        if (Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _activeCamera.nearClipPlane;
        Ray ray = _activeCamera.ScreenPointToRay(mousePos);
        RaycastHit2D
            hit = Physics2D.Raycast(ray.origin, ray.direction, 100, _placementLayers);
        if (hit.collider)
        {
            _lastPosition = hit.point;
        }

        Debug.DrawLine(_activeCamera.transform.position, hit.point);
        return _lastPosition;
    }
}