using System;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    private InputManager _inputManager => GameController.Instance.InputManager;
    private TownObjectManager _townObjectManager => GameController.Instance.TownObjectManager;
    [SerializeField] private GridManager _gridManager;

    [SerializeField] private GameObject _mouseIndicator;
    [SerializeField] private SpriteRenderer _cellIndicator;
    [SerializeField] private Vector3Int _currentCell;
    [SerializeField] private Grid _worldGrid;

    private bool _placementActive;

    private void Start()
    {
        _inputManager.OnSelectPrimary += PlaceObject;
        _inputManager.OnSelectSecondary += RemoveObject;
        _townObjectManager.OnSelectionStateChanged += DoSelectionStateChanged;
    }

    private void DoSelectionStateChanged(bool started)
    {
        _placementActive = started;
        if(started)
            PreviewCell();
    }

    private void Update()
    {
        if(!_placementActive) return;
        PlaceCell();
    }

    private void PlaceCell()
    {
        Vector3 mousePosition = _inputManager.GetSelectedMapPosition();
        _mouseIndicator.transform.position = mousePosition;
        mousePosition.y = Mathf.Ceil(mousePosition.y);
        _currentCell = _worldGrid.LocalToCell(mousePosition);
        _cellIndicator.transform.parent.position = _currentCell;
    }

    private void PreviewCell()
    {
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