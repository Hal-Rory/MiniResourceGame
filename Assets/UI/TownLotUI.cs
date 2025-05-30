using System;
using System.Collections.Generic;
using Controllers;
using Interfaces;
using TMPro;
using Town.TownObjects;
using Town.TownPopulation;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    /// <summary>
    /// Handles the UI for the lots when selected
    /// </summary>
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

        [SerializeField] private TextMeshProUGUI _tooltipHeaderCard;
        [SerializeField] private CardTMP _headerCard;
        [SerializeField] private CardTMP _typeCard;
        [SerializeField] private CardTMP _perksCard;
        [SerializeField] private List<ToggleCardTMP> _capacityCards;

        [SerializeField] private CardTMP _personCard;
        [SerializeField] private CardTMP _shortCard;
        [SerializeField] private Sprite _employeeIcon;
        [SerializeField] private Sprite _visitorIcon;
        [SerializeField] private Sprite _moneyIcon;

        private ToggleCardTMP _currentTooltip;
        private Action UpdateTooltip;

        void Start()
        {
            ForceShutdown();
            GameController.Instance.Selection.OnTownObjectSelected += TownLotSelected;
            GameController.Instance.Selection.OnTownObjectDeselected += TownLotDeselected;
            GameController.Instance.RegisterPlacementListener(OnAddLot, OnRemoveLot);
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
            GameController.Instance.UnregisterPlacementListener(OnAddLot, OnRemoveLot);
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
            if (!_current) return;
            CloseMenu();
        }

        public void DemolishLot_Button()
        {
            GameController.Instance.RemoveLot(_current.CellBlock);
        }

        private void OnAddLot(TownLot lot)
        {
            lot.Popup($"-${lot.GetLotPrice()}", _moneyIcon);
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

        /// <summary>
        /// Exiting the window in a way that informs the selection manager that a lot's selection has been ended
        /// </summary>
        public void FinishAndClose()
        {
            //if this doesn't trigger, there's a problem with the selected lot not being officially selected
            bool closed = GameController.Instance.Selection.TryDeselectTownLot();
        }

        public void ForceShutdown()
        {
            _lotPanel.SetActive(false);
            _cardsScroll.verticalNormalizedPosition = 1;
            CloseTooltip();
            _current = null;
            FinishAndClose();
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

        private void OnWorkplaceUpdated(Workplace lot)
        {
            if (!Active) return;
            UpdateHeaders();
            SetCapacityCards();
            if (!_currentTooltip) return;
            RefreshTooltip(_currentTooltip.ID);
        }

        private void OnPopulationCreated(IPopulation population)
        {
            if (population is not Household household) return;
            GameController.Instance.LotFactory.GetLot(household.HouseID).Popup($"+{household.GetInhabitants().Count}", _visitorIcon);
            if (!Active) return;
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
            string perks = string.Empty;
            if (_current is Workplace workplace)
            {
                perks = $"{workplace.GetWorkplacePerks()}{(_current.CanHaveVisitors() ? "\n" : "")}";
            }
            if(_current.CanHaveVisitors())
            {
                perks += $"{_current.GetLotPerks()}";
            }
            if(!string.IsNullOrEmpty(perks))
            {
                _perksCard.SetLabel(perks);
                _perksCard.gameObject.SetActive(true);
            }
            else
            {
                _perksCard.gameObject.SetActive(false);
            }
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
                _personListView.ClearCards();
                _soundManager.PlayCancel();
                return;
            }
            _soundManager.PlaySelect();
            _personListView.ClearCards();
            _currentTooltip = card as ToggleCardTMP;
            RefreshTooltip(card.ID);
        }

        public void ToggleOffTooltip()
        {
            if (_currentTooltip)
            {
                CloseTooltip();
                _personListView.ClearCards();
                _soundManager.PlayCancel();
            }
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

            _personListView.StashCards();

            _tooltipHeaderCard.text = _current.GetLotName();

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
                personCard.SetEmpty();
                personCard.SetHeader($"{p.Name} ({p.AgeGroup.ToString()})");
                personCard.SetLabel(p.ToString());
                personCard.SetIcon(_employeeIcon);
                UpdateTooltip += () =>
                {
                    personCard.SetHeader(p.Name);
                    personCard.SetLabel(p.ToString());
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
                CardTMP personCard = _personListView.SpawnItem(i.ToString(), _personCard.gameObject)
                    .GetComponent<CardTMP>();
                Person p = house.GetInhabitants()[i];
                personCard.SetEmpty();
                personCard.SetHeader($"{p.Name} ({p.AgeGroup.ToString()})");
                personCard.SetLabel(p.ToString());
                personCard.SetIcon(_visitorIcon);
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
                CardTMP personCard = _personListView.SpawnItem(v.ToString(), _shortCard.gameObject)
                    .GetComponent<CardTMP>();
                Person p = _current.GetVisitors()[v];
                personCard.SetEmpty();
                personCard.SetHeader($"{p.Name} ({p.AgeGroup.ToString()})");
                personCard.SetLabel($"{p.PrintLocation()}" +
                                    $"\n{p.PrintHappiness()}");
                personCard.SetIcon(_visitorIcon);
                UpdateTooltip += () =>
                {
                    personCard.SetHeader($"{p.Name} ({p.AgeGroup.ToString()})");
                    personCard.SetLabel($"{p.PrintLocation()}" +
                                        $"\n{p.PrintHappiness()}");
                };
            }
        }

        private void LateStateClockUpdate(GameTimeManager.TimesOfDay obj)
        {
            if (!Active) return;
            SetCapacityCards();
            if (!_currentTooltip) return;
            RefreshTooltip(_currentTooltip.ID);
        }
    }
}