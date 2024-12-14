using System;
using System.Collections.Generic;
using Controllers;
using Placement;
using Town.TownObjects;
using Town.TownPopulation;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class TownLotUI : MonoBehaviour, IUIControl
    {
        public enum CardTypes
        {
            Perks,
            Employees,
            Visitors,
            Inhabitants
        }

        private SoundManager _soundManager => GameController.Instance.Sound;
        private UIManager _uiManager => GameController.Instance.UI;

        public bool Active { get; set; }

        private TownLot _current;

        [SerializeField] private GameObject _lotPanel;
        [SerializeField] private GameObject _tooltipPanel;
        [SerializeField] private RectTransform _baseCardContainer;

        [SerializeField] private ScrollRect _cardsScroll;
        [SerializeField] private ListView _personListView;
        [SerializeField] private ScrollRect _personScroll;

        [SerializeField] private CardTMP _headerCard;
        [SerializeField] private CardTMP _typeCard;
        [SerializeField] private CardTMP _perksCard;
        [SerializeField] private List<ToggleCardTMP> _capacityCards;

        [SerializeField] private CardTMP _lotCardPrefab;
        [SerializeField] private CardTMP _personCard;

        private string _currentTooltip;
        private Action UpdateTooltip;

        void Start()
        {
            ForceShutdown();
            GameController.Instance.Selection.OnTownObjectSelected += TownLotSelected;
            GameController.Instance.Selection.OnTownObjectDeselected += TownLotDeselected;
            GameController.Instance.RegisterPlacementListener(null, OnRemoveLot);
            GameController.Instance.GameTime.RegisterListener(lateClockUpdate: ClockUpdate, lateStateClockUpdate:LateStateClockUpdate);
        }

        private void TownLotSelected(TownLot lot)
        {
            if (lot == null) return;
            //this stops it from switching while active, remove if the need for that arises
            if (!_uiManager.TrySetActive(this) && !_uiManager.HasControl(this)) return;
            CloseTooltip();
            _current = lot;
            SetDisplay();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_baseCardContainer);
        }

        private void TownLotDeselected(TownLot _)
        {
            CloseMenu();
        }

        public void DemolishLot_Button()
        {
            GameController.Instance.RemoveLot(_current.CellBlock);
        }

        private void OnRemoveLot(TownLot obj)
        {
            CloseTooltip();
            if (_current != obj) return;
            TownLotDeselected(null);
        }

        public void CloseMenu()
        {
            _uiManager.EndControl(this);
        }

        public void ForceShutdown()
        {
            _lotPanel.SetActive(false);
            _cardsScroll.verticalNormalizedPosition = 1;
            CloseTooltip();
            _current = null;
        }

        private void SetDisplay()
        {
            UpdateDisplay();
            SetCapacityCards();
            _lotPanel.SetActive(true);
        }

        private void UpdateDisplay()
        {
            _headerCard.SetHeader(_current.GetLotName());
            _headerCard.SetIcon(_current.GetLotDepiction());
            _typeCard.SetLabel($"{_current.LotType}");
            _perksCard.SetLabel($"{_current.GetLotPerks()}");
        }

        private void SetCapacityCards()
        {
            foreach (ToggleCardTMP card in _capacityCards)
            {
                card.gameObject.SetActive(false);
                switch (_current)
                {
                    case not House when card.ID == CardTypes.Visitors.ToString():
                        card.SetLabel(_current.ColoredVisitorText());
                        card.gameObject.SetActive(_current.CanHaveVisitors());
                        card.Interactable = _current.CanHaveVisitors() && _current.GetVisitorCount() > 0;
                        card.gameObject.SetActive(true);
                        break;
                    case Workplace workplace when card.ID == CardTypes.Employees.ToString():
                        card.SetLabel(workplace.ColoredEmployeeText());
                        card.Interactable = workplace.EmployeeCount > 0;
                        card.gameObject.SetActive(true);
                        break;
                    case House house when card.ID == CardTypes.Inhabitants.ToString():
                        card.SetLabel(house.ColoredInhabitantsText());
                        card.Interactable = house.GetVisitorCount() > 0;
                        card.gameObject.SetActive(true);
                        break;
                }
            }
        }

        public void OpenTooltip_UI(CardTMP card)
        {
            if (_currentTooltip == card.ID)
            {
                CloseTooltip();
                _soundManager.PlayCancel();
                return;
            }

            _soundManager.PlaySelect();
            _currentTooltip = card.ID;
            RefreshTooltip(card.ID);
        }

        private void RefreshTooltip(string card)
        {
            UpdateTooltip = null;
            _personListView.ClearCards();

            if (card == CardTypes.Employees.ToString())
            {
                UpdateEmployeeTooltip();
            }

            if (card == CardTypes.Inhabitants.ToString())
            {
                UpdateHouseTooltip();
            }

            if (card == CardTypes.Visitors.ToString())
            {
                UpdateLotTooltip();
            }
            _personListView.UpdateLayout();
            _tooltipPanel.SetActive(true);
        }

        private void UpdateLotTooltip()
        {
            if (_current.GetVisitorCount() == 0) return;
            for (int v = 0; v < _current.GetVisitorCount(); v++)
            {
                CardTMP personCard = _personListView.SpawnItem(v.ToString(), _personCard.gameObject)
                    .GetComponent<CardTMP>();
                personCard.SetLabel(_current.GetVisitors()[v].Name);
            }
        }

        private void UpdateHouseTooltip()
        {
            if (_current is not House house) return;
            if (_current.GetVisitorCount() == 0) return;

            for (int i = 0; i < house.GetVisitorCount(); i++)
            {
                CardTMP personCard = _personListView.SpawnItem(i.ToString(), _lotCardPrefab.gameObject)
                    .GetComponent<CardTMP>();
                Person p = house.GetVisitors()[i];
                personCard.SetHeader(p.Name);
                personCard.SetLabel(p.ToString());
                UpdateTooltip += () =>
                {
                    personCard.SetLabel(p.ToString());
                };
            }
        }

        private void UpdateEmployeeTooltip()
        {
            if (_current is not Workplace workplace) return;
            if (workplace.EmployeeCount == 0) return;
            for (int e = 0; e < workplace.EmployeeCount; e++)
            {
                CardTMP personCard = _personListView.SpawnItem(e.ToString(), _personCard.gameObject)
                    .GetComponent<CardTMP>();
                personCard.SetLabel(workplace.GetEmployees()[e].Name);
            }
        }

        private void CloseTooltip()
        {
            _currentTooltip = string.Empty;
            _personScroll.verticalNormalizedPosition = 1;
            _tooltipPanel.SetActive(false);
        }

        private void ClockUpdate(int tick)
        {
            if (!_current || !Active) return;
            UpdateDisplay();
            SetCapacityCards();
            if(_tooltipPanel.activeSelf) RefreshTooltip(_currentTooltip);
        }

        private void LateStateClockUpdate(GameTimeManager.TimesOfDay obj)
        {
            if (!_tooltipPanel.activeSelf && _currentTooltip != CardTypes.Inhabitants.ToString()) return;
            UpdateTooltip?.Invoke();
        }
    }
}