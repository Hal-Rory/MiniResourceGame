using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    private GameController _controller => GameController.Instance;
    private InputManager _input => _controller.Input;
    private TownObjectManager _townObject => _controller.TownObject;

    [SerializeField] private GameObject _mouseIndicator;
    [SerializeField] private SpriteRenderer _cellIndicator;
    [SerializeField] private Vector3Int _currentCell;
    [SerializeField] private Grid _worldGrid;
    private bool _canPlace;
    private bool _placementActive => _controller.PlacementMode;
    private void Start()
    {
        PreviewCell(false);
        _townObject.OnStateChanged += DoStateChanged;
    }
    private void Update()
    {
        if(!_placementActive) return;
        PlaceCell();
        if (_input.PrimaryPressed)
        {
            PlaceObject();
        }

        if (_input.SecondaryPressed)
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
        Vector3 mousePosition = _input.GetSelectionPosition(1, out Collider2D col);
        _canPlace = col != null;
        _mouseIndicator.transform.position = mousePosition;
        mousePosition.y = Mathf.Ceil(mousePosition.y);
        _currentCell = _worldGrid.LocalToCell(mousePosition);
        _cellIndicator.transform.parent.position = _currentCell;
    }

    private void PreviewCell(bool visible)
    {
        _cellIndicator.gameObject.SetActive(visible);
        if (!visible) return;
        Vector2Int size = _townObject.CurrentObject ? _townObject.CurrentObject.LotSize: Vector2Int.one;
        if(size.magnitude > 1)
        {
            _cellIndicator.size = size;
        }
    }

    private void PlaceObject()
    {
        if (_input.IsPointerOverUI() || !_placementActive || _townObject.CurrentObject == null || !_canPlace) return;
        _controller.PlaceLot(_townObject.CurrentObject, _currentCell);
    }

    private void RemoveObject()
    {
        if (!_placementActive || _townObject.CurrentObject != null) return;
        _controller.RemoveLot(_currentCell);
    }
}