using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlacementSystem : MonoBehaviour
{
    private InputManager _inputManager => GameController.Instance.InputManager;
    private TownObjectManager _townObjectManager => GameController.Instance.TownObjectManager;
    [SerializeField] private GridManager _gridManager;

    [SerializeField] private GameObject _mouseIndicator;
    [SerializeField] private SpriteRenderer _cellIndicator;
    [SerializeField] private Vector3Int _currentCell;
    [SerializeField] private Grid _worldGrid;
    private bool _placementActive => GameController.Instance.PlacementMode;
    private void Start()
    {
        PreviewCell(false);
        _townObjectManager.OnStateChanged += DoStateChanged;
    }
    private void Update()
    {
        if(!_placementActive) return;
        PlaceCell();
        if (_inputManager.PrimaryPressed)
        {
            PlaceObject();
        }

        if (_inputManager.SecondaryPressed)
        {
            RemoveObject();
        }
    }

    private void DoStateChanged(bool started)
    {
        PreviewCell(started);
    }

    private void PlaceCell()
    {
        Vector3 mousePosition = _inputManager.GetSelectionPosition(1, out Collider2D _);
        _mouseIndicator.transform.position = mousePosition;
        mousePosition.y = Mathf.Ceil(mousePosition.y);
        _currentCell = _worldGrid.LocalToCell(mousePosition);
        _cellIndicator.transform.parent.position = _currentCell;
    }

    private void PreviewCell(bool visible)
    {
        _cellIndicator.gameObject.SetActive(visible);
        if (!visible) return;
        Vector2Int size = _townObjectManager.CurrentObject ? _townObjectManager.CurrentObject.LotSize: Vector2Int.one;
        if(size.magnitude > 1)
        {
            _cellIndicator.size = size;
        }
    }

    private void PlaceObject()
    {
        if (_inputManager.IsPointerOverUI() || !_placementActive || _townObjectManager.CurrentObject == null) return;
        _gridManager.AddLot(_townObjectManager.CurrentObject, _currentCell);
    }

    private void RemoveObject()
    {
        if (!_placementActive || _townObjectManager.CurrentObject != null) return;
        _gridManager.RemoveLot(_currentCell);
    }
}