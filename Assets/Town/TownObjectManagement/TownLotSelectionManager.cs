using System;
using UnityEngine;

public class TownLotSelectionManager : MonoBehaviour
{
    private InputManager _inputManager => GameController.Instance.InputManager;
    public event Action<TownLot> OnTownObjectSelected;
    public event Action<TownLot> OnTownObjectDeselected;
    private TownLot _lastFound;
    private bool _selected;

    private void Update()
    {
        TryHover();
        if (_inputManager.ExitPressed || (_inputManager.SecondaryPressed && !_inputManager.IsPointerOverUI()))
        {
            DeselectTownLot();
        }

        if (_inputManager.PrimaryPressed)
        {
            SelectTownLot();
        }
    }

    private void DeselectTownLot()
    {
        _selected = false;
        if (!_lastFound) return;
        OnTownObjectDeselected?.Invoke(_lastFound);
    }


    private void TryHover()
    {
        if (_inputManager.IsPointerOverUI() || GameController.Instance.PlacementMode || _selected) return;

        _inputManager.GetSelectionPosition(1 << LayerMask.NameToLayer("TownLot"), out Collider2D col);

        if (!col || !(col.transform.parent.TryGetComponent(out TownLot lot) && lot))
        {
            if (!_lastFound) return;
            _lastFound.EndHovering();
            _lastFound = null;
        }
        else
        {
            if (lot == _lastFound) return;
            if(_lastFound) _lastFound.EndHovering();
            _lastFound = lot;
            lot.StartHovering();
        }
    }

    private void SelectTownLot()
    {
        if (_inputManager.IsPointerOverUI() || GameController.Instance.PlacementMode || !_lastFound) return;
        OnTownObjectSelected?.Invoke(_lastFound);
        _selected = true;
    }
}