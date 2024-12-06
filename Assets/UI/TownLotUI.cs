using System;
using System.Collections.Generic;
using System.Linq;
using Common.UI;
using Controllers;
using Placement;
using Town.TownObjects;
using Town.TownPopulation;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TownLotUI : MonoBehaviour, IUIControl
    {
        private enum CardTypes
        {
            Type,
            Criteria,
            Perks,
            Employees,
            Patrons,
            Inhabitants
        }

        private TownLot _current;

        [SerializeField] private GameObject _lotPanel;
        [SerializeField] private GameObject _tooltipPanel;
        [SerializeField] private CardTMP _headerCard;
        [SerializeField] private CardTMP _typeCard;
        [SerializeField] private List<CardTMP> _baseCards;

        [SerializeField] private List<CardTMP> _capacityCards;

        [SerializeField] private CardTMP _lotCardPrefab;
        [SerializeField] private ListView _housingCardsList;
        [SerializeField] private CardTMP _personCard;
        [SerializeField] private ListView _personListView;

        private string _currentTooltip;
        private Dictionary<string, Toggle> _tooltipToggles;

        public bool Active { get; set; }

        private WorkplaceManager _workplaceManager => GameController.Instance.Workplace;

        void Start()
        {
            ForceShutdown();
            GameController.Instance.Selection.OnTownObjectSelected += TownLotSelected;
            GameController.Instance.Selection.OnTownObjectDeselected += TownLotDeselected;
            GameController.Instance.RegisterPlacementListener(null, OnRemoveLot);
            GameController.Instance.GameTime.RegisterListener(lateClockUpdate: ClockUpdate);
            _workplaceManager.OnWorkforceChanged += OnWorkforceChanged;
            _tooltipToggles = new Dictionary<string, Toggle>();
            foreach (CardTMP card in _baseCards)
            {
                if (card.ID == CardTypes.Employees.ToString() || card.ID == CardTypes.Patrons.ToString() || card.ID == CardTypes.Inhabitants.ToString())
                {
                    _tooltipToggles.Add(card.ID, card.gameObject.GetComponentInChildren<Toggle>());
                }
            }
        }

        private void OnWorkforceChanged()
        {
            if (!Active || !_current || _current is not Workplace workplace) return;
            CardTMP employeeCard = _baseCards.Find(card => card.ID == CardTypes.Employees.ToString());
            if (!employeeCard) return;
            employeeCard.SetLabel($"<color=#F3B41B>{workplace.EmployeeCount}</color> / {workplace.MaxEmployeeCapacity}");
            _tooltipToggles[employeeCard.ID].interactable = workplace.GetEmployees().Count > 0;
            if (!_tooltipPanel.activeSelf) return;
                UpdateEmployeeTooltip();
        }

        private void OnRemoveLot(TownLot obj)
        {
            if (_current != obj) return;
            TownLotDeselected(null);
        }

        public void ForceShutdown()
        {
            _lotPanel.SetActive(false);
            _tooltipPanel.SetActive(false);
            _current = null;
        }

        private void SetDisplay(TownLot lot)
        {
            _headerCard.SetLabel(lot.GetName());
            _headerCard.SetIcon(lot.GetDepiction());
            _typeCard.SetLabel($"{lot.LotType}");
            _lotPanel.SetActive(true);
        }
        private void SetDisplay(Workplace workplace)
        {
            foreach (CardTMP card in _baseCards)
            {
                card.gameObject.SetActive(true);
                if (card.ID == CardTypes.Criteria.ToString())
                {
                    card.SetLabel($"{workplace.GetWorkCriteria()}");
                } else if (card.ID == CardTypes.Perks.ToString())
                {
                    card.SetLabel($"{workplace.Wages} / mo\n{workplace.GetHappiness()} mood / d");
                } else if (card.ID == CardTypes.Employees.ToString())
                {
                    card.SetLabel($"<color=#F3B41B>{workplace.EmployeeCount}</color> / {workplace.MaxEmployeeCapacity}");
                    _tooltipToggles[card.ID].interactable = workplace.EmployeeCount > 0;
                }
                else if (card.ID == CardTypes.Patrons.ToString())
                {
                    card.SetLabel($"[{workplace.GetPatronCriteria()}]\n<color=#F3B41B>{workplace.GetPersons().Count}</color> / {workplace.GetMaxCapacity()}");
                    _tooltipToggles[card.ID].interactable = workplace.GetPersons().Count > 0;
                }
                else
                {
                    card.gameObject.SetActive(false);
                }
            }
        }
        private void SetDisplay(House house)
        {
            foreach (CardTMP card in _baseCards)
            {
                if (card.ID == CardTypes.Inhabitants.ToString())
                {
                    card.gameObject.SetActive(true);
                    card.SetLabel($"<color=#F3B41B>{house.GetPersons().Count}</color> / {house.GetMaxCapacity()}");
                }
                else
                {
                    card.gameObject.SetActive(false);
                }
            }
            List<Person> inhabitants = house.GetPersons();
            if (inhabitants == null || inhabitants.Count == 0) return;
            _personListView.ClearCards();
            for (int p = 0; p < inhabitants.Count; p++)
            {
                CardTMP personCard = _personListView.SpawnItem(p.ToString(), _lotCardPrefab.gameObject)
                    .GetComponent<CardTMP>();
                personCard.SetHeader(inhabitants[p].Name);
                personCard.SetLabel(inhabitants[p].ToString());
            }
            _personListView.UpdateLayout();
        }

        private void TownLotDeselected(TownLot _)
        {
            CloseMenu();
        }

        public void CloseMenu()
        {
            GameController.Instance.UI.EndControl(this);
        }

        private void TownLotSelected(TownLot lot)
        {
            if (lot == null) return;
            if (!GameController.Instance.UI.TrySetActive(this)) return;
            switch (lot)
            {
                case Workplace workplace:
                    SetDisplay(workplace);
                    break;
                case House house:
                    SetDisplay(house);
                    break;
            }
            _current = lot;
            SetDisplay(lot);
        }

        public void DemolishLot_Button()
        {
            GameController.Instance.RemoveLot(_current.CellBlock);
        }

        public void OpenTooltip_UI(CardTMP card)
        {
            if (_currentTooltip == card.ID)
            {
                CloseTooltip();
                return;
            }

            _currentTooltip = card.ID;
            bool canOpen = false;
            if (card.ID == CardTypes.Employees.ToString())
            {
                canOpen = UpdateEmployeeTooltip();
            }
            _tooltipPanel.SetActive(canOpen);
        }

        private bool UpdateEmployeeTooltip()
        {
            if (_current is not Workplace workplace) return false;
            List<Person> employees = workplace.GetEmployees();
            if (employees.Count != 0)
            {
                _personListView.ClearCards();
                for (int p = 0; p < employees.Count; p++)
                {
                    CardTMP personCard = _personListView.SpawnItem(p.ToString(), _personCard.gameObject)
                        .GetComponent<CardTMP>();
                    personCard.SetLabel(employees[p].Name);
                }
                _personListView.UpdateLayout();
            }
            return employees.Count != 0;
        }

        private void CloseTooltip()
        {
            _currentTooltip = string.Empty;
            _tooltipPanel.SetActive(false);
        }

        private void ClockUpdate(int tick)
        {
            if (!_current) return;
            string capacity = $"<color=#F3B41B>{(_current.GetPersons() != null ? _current.GetPersons().Count : 0)}</color> / {_current.GetMaxCapacity()}";
            _capacityCards.ForEach(card =>
            {
                card.SetLabel(card.ID == CardTypes.Patrons.ToString()
                    ? $"[{_current.GetPatronCriteria()}]\n{capacity}"
                    : $"{capacity}");
                if (_tooltipToggles.TryGetValue(card.ID, out Toggle toggle))
                {
                    toggle.interactable = _current.GetPersons().Count > 0;
                }
            });
        }
    }
}