using System;
using System.Collections.Generic;
using Controllers;
using Interfaces;
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
        [SerializeField] private Sprite _employeeIcon;
        [SerializeField] private Sprite _visitorIcon;

        private ToggleCardTMP _currentTooltip;
        private Action UpdateTooltip;

        void Start()
        {
            ForceShutdown();
            GameController.Instance.Selection.OnTownObjectSelected += TownLotSelected;
            GameController.Instance.Selection.OnTownObjectDeselected += TownLotDeselected;
            GameController.Instance.RegisterPlacementListener(null, OnRemoveLot);
            GameController.Instance.GameTime.RegisterListener(lateStateClockUpdate:LateStateClockUpdate);
            GameController.Instance.Population.OnPopulationCreated += OnPopulationCreated;
            GameController.Instance.Workplace.OnWorkplaceUpdated += OnWorkplaceUpdated;
        }

        private void OnDestroy()
        {
            if (!GameController.Instance) return;
            if(GameController.Instance.Selection)
            {
                GameController.Instance.Selection.OnTownObjectSelected -= TownLotSelected;
                GameController.Instance.Selection.OnTownObjectDeselected -= TownLotDeselected;
            }
            GameController.Instance.UnregisterPlacementListener(null, OnRemoveLot);
            if(GameController.Instance.GameTime) GameController.Instance.GameTime.UnregisterListener(lateStateClockUpdate:LateStateClockUpdate);
            if(GameController.Instance.Population != null) GameController.Instance.Population.OnPopulationCreated -= OnPopulationCreated;
            if(GameController.Instance.Workplace != null) GameController.Instance.Workplace.OnWorkplaceUpdated -= OnWorkplaceUpdated;
        }

        #region Townlots
        private void TownLotSelected(TownLot lot)
        {
            if (lot == null) return;
            //this stops it from switching while active, remove if the need for that arises
            if (!_uiManager.TrySetActive(this) && !_uiManager.HasControl(this)) return;
            _soundManager.PlaySelect();
            CloseTooltip();
            _current = lot;
            UpdateHeaders();
            SetCapacityCards();
            _lotPanel.SetActive(true);
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
        #endregion

        #region Menu controls
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

        /// <summary>
        /// Close the tooltip, clear the current tooltip card, and reset the view position
        /// </summary>
        private void CloseTooltip()
        {
            _currentTooltip = null;
            _personScroll.verticalNormalizedPosition = 1;
            _tooltipPanel.SetActive(false);
        }
        #endregion

        private void OnWorkplaceUpdated(Workplace obj)
        {
            UpdateHeaders();
            SetCapacityCards();
            if (!_currentTooltip) return;
            RefreshTooltip(_currentTooltip.ID);
        }

        private void OnPopulationCreated(IPopulation population)
        {
            if (!Active || population is not Household) return;
            UpdateHeaders();
            SetCapacityCards();
            if (!_currentTooltip) return;
            RefreshTooltip(_currentTooltip.ID);
        }

        /// <summary>
        /// Will update the primary header cards
        /// (Header, Type, Perks)
        /// </summary>
        private void UpdateHeaders()
        {
            _headerCard.SetHeader(_current.GetLotName());
            _headerCard.SetIcon(_current.GetLotDepiction());
            _typeCard.SetLabel($"{_current.LotType}");
            _perksCard.SetLabel($"{_current.GetLotPerks()}");
        }

        /// <summary>
        /// Iterates through the capacity cards to set them according to the lot's data
        /// (Employee count, inhabitants count, visitor count(if applicable))
        /// </summary>
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
                        break;
                    case Workplace workplace when card.ID == CardTypes.Employees.ToString():
                        card.SetLabel(workplace.ColoredEmployeeText());
                        card.gameObject.SetActive(true);
                        break;
                    case House house when card.ID == CardTypes.Inhabitants.ToString():
                        card.SetLabel(house.ColoredInhabitantsText());
                        card.gameObject.SetActive(true);
                        break;
                }
            }
        }

        /// <summary>
        /// The UI toggles will open or close the tooltip
        /// depending on which card is being passed in
        /// </summary>
        /// <param name="card"></param>
        public void OpenTooltip_UI(CardTMP card)
        {
            if (_currentTooltip && _currentTooltip.ID == card.ID)
            {
                CloseTooltip();
                _soundManager.PlayCancel();
                return;
            }
            _soundManager.PlaySelect();
            _currentTooltip = card as ToggleCardTMP;
            RefreshTooltip(card.ID);
        }

        /// <summary>
        /// Depending on the card ID this will update the tool tips for that type
        /// (Employees, Inhabitants, Visitors (if applicable))
        /// by recreating the person list view completely
        /// </summary>
        /// <param name="card"></param>
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

        /// <summary>
        /// Create the list view using the employee list
        /// </summary>
        private void UpdateEmployeeTooltip()
        {
            if (_current is not Workplace workplace) return;
            if (workplace.EmployeeCount == 0) return;
            for (int e = 0; e < workplace.EmployeeCount; e++)
            {
                CardTMP personCard = _personListView.SpawnItem(e.ToString(), _personCard.gameObject)
                    .GetComponent<CardTMP>();
                Person p = workplace.GetEmployees()[e];
                personCard.SetLabel(p.Name);
                personCard.SetIcon(_employeeIcon);
                UpdateTooltip += () =>
                {
                    personCard.SetLabel(p.Name);
                };
            }
        }

        /// <summary>
        /// Create the list view using the inhabitants list
        /// </summary>
        private void UpdateHouseTooltip()
        {
            if (_current is not House house) return;
            for (int i = 0; i < house.GetInhabitantsCount(); i++)
            {
                CardTMP personCard = _personListView.SpawnItem(i.ToString(), _lotCardPrefab.gameObject)
                    .GetComponent<CardTMP>();
                Person p = house.GetInhabitants()[i];
                personCard.SetHeader(p.Name);
                personCard.SetLabel(p.ToString());
                UpdateTooltip += () =>
                {
                    personCard.SetLabel(p.ToString());
                };
            }
        }

        /// <summary>
        /// Create the list view using the visitors list
        /// </summary>
        private void UpdateLotTooltip()
        {
            if (_current.GetVisitorCount() == 0) return;
            for (int v = 0; v < _current.GetVisitorCount(); v++)
            {
                CardTMP personCard = _personListView.SpawnItem(v.ToString(), _personCard.gameObject)
                    .GetComponent<CardTMP>();
                Person p = _current.GetVisitors()[v];
                personCard.SetLabel(p.Name);
                personCard.SetIcon(_visitorIcon);
                UpdateTooltip += () =>
                {
                    personCard.SetLabel(p.Name);
                };
            }
        }

        private void LateStateClockUpdate(GameTimeManager.TimesOfDay obj)
        {
            if (!Active || !_currentTooltip) return;
            SetCapacityCards();
            RefreshTooltip(_currentTooltip.ID);
        }
    }
}