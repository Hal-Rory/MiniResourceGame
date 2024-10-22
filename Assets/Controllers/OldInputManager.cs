using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OldInputManager : MonoBehaviour
{
    [SerializeField] private Camera _activeCamera;
    private Vector3 _lastPosition;
    public event Action OnSelectPrimary;
    public event Action OnSelectSecondary;
    public event Action OnExit;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnSelectPrimary?.Invoke();
        if (Input.GetMouseButtonDown(1))
            OnSelectSecondary?.Invoke();
        if (Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectionPosition(LayerMask layer, out Collider2D col)
    {
        Vector3 mousePos = Input.mousePosition;
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
}