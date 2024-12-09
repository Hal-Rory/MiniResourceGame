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
        if (_input.IsPointerOverUI() || GameController.Instance.PlacementMode) return;
        OnTownObjectSelected?.Invoke(_lastFound);
    }

    public void DeselectTownLot()
    {
        OnTownObjectDeselected?.Invoke(null);
    }

    private void TryHover()
    {
        if (_input.IsPointerOverUI() || GameController.Instance.PlacementMode) return;

        _input.GetSelectionPosition(1 << LayerMask.NameToLayer("TownLot"), out Collider2D col);

        if (col && col.transform.parent.TryGetComponent(out TownLot lot) && lot)
        {
            if (lot == _lastFound) return;
            if(_lastFound) _lastFound.EndHovering();
            _lastFound = lot;
            lot.StartHovering();
            return;
        }
        if (_lastFound) _lastFound.EndHovering();
        _lastFound = null;
    }
}