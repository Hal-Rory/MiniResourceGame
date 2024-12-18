using Controllers;
using UnityEngine;

namespace Placement
{
    public class PlacementSystem : MonoBehaviour
    {
        private GameController _controller => GameController.Instance;
        private InputManager _input => _controller.Input;
        private TownObjectManager _townObject => _controller.TownObject;

        [SerializeField] private Transform _cellIndicator;
        [SerializeField] private SpriteRenderer _cellOffset;
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
            mousePosition.y = Mathf.Ceil(mousePosition.y);
            _currentCell = _worldGrid.LocalToCell(mousePosition);
            _cellIndicator.position = _currentCell;
        }

        private void PreviewCell(bool visible)
        {
            _cellIndicator.gameObject.SetActive(visible);
            if (!visible) return;
            Vector2Int size = Vector2Int.one;
            Vector3 placement = new Vector3(.5f, -.5f);
            if (_townObject.CurrentObject)
            {
                size = _townObject.CurrentObject.LotSize;
                placement = new Vector3(
                    _townObject.CurrentObject.LotSize.x * .5f,
                    _townObject.CurrentObject.LotSize.y * -.5f,
                    0);
            }
            _cellOffset.size = size;
            _cellOffset.transform.localPosition = placement;
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
}