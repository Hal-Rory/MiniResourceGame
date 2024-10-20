using System;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    private InputManager _inputManager => GameController.Instance.InputManager;
    private ObjectManager _objectManager => GameController.Instance.ObjectManager;
    [SerializeField] private GridManager _gridManager;

    [SerializeField] private GameObject _mouseIndicator;
    [SerializeField] private SpriteRenderer _cellIndicator;
    [SerializeField] private Vector3Int _currentCell;
    [SerializeField] private Grid _worldGrid;

    private void Start()
    {
        _inputManager.OnSelectPrimary += PlaceObject;
        _inputManager.OnSelectSecondary += RemoveObject;
    }

    private void Update()
    {
        PlaceCell();
    }

    private void PlaceCell()
    {
        Vector3 mousePosition = _inputManager.GetSelectedMapPosition();
        _mouseIndicator.transform.position = mousePosition;
        mousePosition.y = Mathf.Ceil(mousePosition.y);
        _currentCell = _worldGrid.LocalToCell(mousePosition);
        _cellIndicator.transform.parent.position = _currentCell;
        PreviewCell();
    }

    private void PreviewCell()
    {
        if (!_objectManager.CurrentObject) return;
        Vector2Int size = _objectManager.CurrentObject.LotSize;
        if(size.x > 0 || size.y > 0)
        {
            _cellIndicator.size = size;
        }
    }

    private void PlaceObject()
    {
        if (_objectManager.CurrentObject == null) return;
        _gridManager.AddLot(_objectManager.CurrentObject, _currentCell);
    }

    private void RemoveObject()
    {
        _gridManager.RemoveLot(_currentCell);
    }
}