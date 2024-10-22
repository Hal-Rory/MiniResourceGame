using System;
using UnityEngine;

public class TownLotSelectionManager : MonoBehaviour
{
    private InputManager _inputManager => GameController.Instance.InputManager;
    public event Action<TownLot> OnTownObjectSelected;
    public event Action<TownLot> OnTownObjectDeselected;
    private TownLot _lastSelected;
    void Start()
    {
        _inputManager.OnSelectPrimary += SelectTownLot;
        _inputManager.OnSelectSecondary += DeselectTownLot;
    }

    private void DeselectTownLot()
    {
        if (!_lastSelected) return;
        OnTownObjectDeselected?.Invoke(_lastSelected);
    }

    private void SelectTownLot()
    {
        if (_inputManager.IsPointerOverUI() || GameController.Instance.PlacementMode) return;
        _inputManager.GetSelectionPosition(1<<LayerMask.NameToLayer("TownLot"), out Collider2D col);
        if (!col || !(col.transform.parent.TryGetComponent(out TownLot lot) && lot)) return;
        _lastSelected = lot;
        OnTownObjectSelected?.Invoke(_lastSelected);
    }
}