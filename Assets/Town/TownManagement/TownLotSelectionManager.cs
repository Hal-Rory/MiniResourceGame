using System;
using Controllers;
using Town.TownObjects;
using UnityEngine;

namespace Town.TownManagement
{
    public class TownLotSelectionManager : MonoBehaviour
    {
        private InputManager _input => GameController.Instance.Input;
        public event Action<TownLot> OnTownObjectSelected;
        public event Action<TownLot> OnTownObjectDeselected;
        private TownLot _lastSelected;
        private TownLot _lastHovered;

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
            if (GameController.Instance.PlacementMode) return;
            if (_input.ExitPressed)
            {
                DeselectTownLot();
            }

            if (_input.PrimaryPressed)
            {
                SelectTownLot();
            }
            TryHover();
        }

        public void SelectTownLot()
        {
            if (_input.IsPointerOverUI() || GameController.Instance.PlacementMode) return;
            if (_lastHovered)
            {
                if (_lastSelected == _lastHovered) return;
                if(_lastSelected) _lastSelected.Deselect();
                _lastSelected = _lastHovered;
                _lastSelected.Select();
                OnTownObjectSelected?.Invoke(_lastSelected);
            }
        }

        public void DeselectTownLot()
        {
            if (_lastSelected)
            {
                _lastSelected.Deselect();
                _lastSelected.EndHovering();
            }
            _lastSelected = null;
            OnTownObjectDeselected?.Invoke(null);
        }

        public bool TryDeselectTownLot()
        {
            if(_lastSelected)
            {
                _lastSelected.Deselect();
                _lastSelected.EndHovering();
                _lastSelected = null;
            }
            OnTownObjectDeselected?.Invoke(null);
            return true;
        }

        private void TryHover()
        {
            if (_input.IsPointerOverUI() || GameController.Instance.PlacementMode) return;
            _input.GetSelectionPosition(1 << LayerMask.NameToLayer("TownLot"), out Collider2D col);

            //found target lot
            if (col && col.transform.parent.TryGetComponent(out TownLot lot) && lot)
            {
                //if it's the same lot, stop
                if (lot == _lastHovered || !lot) return;
                if(_lastHovered) _lastHovered.EndHovering();
                _lastHovered = lot;
                _lastHovered.StartHovering();
            }
            else
            {
                if(_lastHovered) _lastHovered.EndHovering();
                _lastHovered = null;
            }
        }
    }
}