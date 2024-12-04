using System;
using Controllers;
using Placement;
using UnityEngine;

public class TownLotSelectionManager : MonoBehaviour
{
    private InputManager _input => GameController.Instance.Input;
    public event Action<TownLot> OnTownObjectSelected;
    public event Action<TownLot> OnTownObjectDeselected;
    private TownLot _lastFound;
    private bool _selected;

    private void Start()
    {
        GameController.Instance.RegisterPlacementListener(null, OnRemoveLot);
    }

    private void OnRemoveLot(TownLot obj)
    {
        DeselectTownLot();
    }


    private void Update()
    {
        TryHover();
        if (_input.ExitPressed)
        {
            DeselectTownLot();
        }

        if (_input.PrimaryPressed)
        {
            SelectTownLot();
        }
    }

    public void SelectTownLot()
    {
        if (_input.IsPointerOverUI() || GameController.Instance.PlacementMode || !_lastFound) return;
        OnTownObjectSelected?.Invoke(_lastFound);
        _selected = true;
    }

    public void DeselectTownLot()
    {
        _selected = false;
        if (!_lastFound) return;
        OnTownObjectDeselected?.Invoke(_lastFound);
    }


    private void TryHover()
    {
        if (_input.IsPointerOverUI() || GameController.Instance.PlacementMode || _selected) return;

        _input.GetSelectionPosition(1 << LayerMask.NameToLayer("TownLot"), out Collider2D col);

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
}