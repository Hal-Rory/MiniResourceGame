using System.Collections.Generic;
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
        public enum CardTypes
        {
            Perks,
            Employees,
            Patrons,
            Inhabitants
        }

        private SoundManager _soundManager => GameController.Instance.Sound;
        private TownLot _current;

        [SerializeField] private GameObject _lotPanel;
        [SerializeField] private GameObject _tooltipPanel;
        [SerializeField] private CardTMP _headerCard;
        [SerializeField] private CardTMP _typeCard;
        [SerializeField] private List<CardTMP> _baseCards;
        [SerializeField] private RectTransform _baseCardContainer;

        [SerializeField] private List<CardTMP> _capacityCards;

        [SerializeField] private CardTMP _lotCardPrefab;
        [SerializeField] private ListView _housingCardsList;
        [SerializeField] private CardTMP _personCard;
        [SerializeField] private ListView _personListView;

        private string _currentTooltip;
        private Dictionary<string, Toggle> _tooltipToggles;

        private UIManager _uiManager => GameController.Instance.UI;

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
            string colorHex = ColorUtility.ToHtmlStringRGB(_uiManager.ColorPalette.Positive);
            employeeCard.SetLabel($"<color=#{colorHex}>{workplace.EmployeeCount}</color> / {workplace.MaxEmployeeCapacity}");
            _tooltipToggles[employeeCard.ID].interactable = workplace.GetEmployees().Count > 0;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_baseCardContainer);
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

        private void SetDisplay()
        {
            _headerCard.SetLabel(_current.GetName());
            _headerCard.SetIcon(_current.GetDepiction());
            _typeCard.SetLabel($"{_current.LotType}");
            _lotPanel.SetActive(true);
            foreach (CardTMP card in _baseCards)
            {
                card.gameObject.SetActive(false);
                if (_current is Workplace workplace)
                {
                    if (card.ID == CardTypes.Employees.ToString())
                    {
                        card.SetLabel($"[{workplace.GetWorkCriteria()}]\n<color=#{_uiManager.ColorPalette.PositiveHex}>{workplace.EmployeeCount}</color> / {workplace.MaxEmployeeCapacity}");
                        _tooltipToggles[card.ID].interactable = workplace.EmployeeCount > 0;
                        card.gameObject.SetActive(true);
                    }
                }
                if (_current is House house)
                {
                    if (card.ID == CardTypes.Inhabitants.ToString())
                    {
                        card.SetLabel($"<color=#{_uiManager.ColorPalette.PositiveHex}>{house.GetPersonsCount()}</color> / {house.GetMaxCapacity()}");
                        _tooltipToggles[card.ID].interactable = house.GetPersonsCount() > 0;
                        card.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (card.ID == CardTypes.Patrons.ToString())
                    {
                        card.SetLabel($"[{_current.GetPatronCriteria()}]\n<color=#{_uiManager.ColorPalette.PositiveHex}>{_current.GetPersonsCount()}</color> / {_current.GetMaxCapacity()}");
                        _tooltipToggles[card.ID].interactable = _current.GetPersonsCount() > 0;
                        card.gameObject.SetActive(true);
                    }
                }

                if (card.ID == CardTypes.Perks.ToString())
                {
                    card.SetLabel($"{_current.GetPerks()}");
                    card.gameObject.SetActive(true);
                }
            }
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
            //this stops it from switching while active, remove if the need for that arises
            if (!GameController.Instance.UI.TrySetActive(this)) return;
            _current = lot;
            SetDisplay();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_baseCardContainer);
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
                _soundManager.PlayCancel();
                return;
            }

            _soundManager.PlaySelect();
            _currentTooltip = card.ID;
            _personListView.ClearCards();

            if (card.ID == CardTypes.Employees.ToString())
            {
                UpdateEmployeeTooltip();
            }

            if (card.ID == CardTypes.Inhabitants.ToString())
            {
                UpdateHouseTooltip();
            }

            if (card.ID == CardTypes.Patrons.ToString())
            {
                UpdateLotTooltip();
            }
        }

        private void UpdateLotTooltip()
        {
            List<Person> patrons = _current.GetPersons();
            if (patrons.Count == 0) return;
            for (int p = 0; p < patrons.Count; p++)
            {
                CardTMP personCard = _personListView.SpawnItem(p.ToString(), _personCard.gameObject)
                    .GetComponent<CardTMP>();
                personCard.SetLabel(patrons[p].Name);
            }
            _personListView.UpdateLayout();
            _tooltipPanel.SetActive(true);
        }

        private void UpdateHouseTooltip()
        {
            if (_current is not House house) return;
            List<Person> inhabitants = house.GetPersons();
            if (inhabitants == null || inhabitants.Count == 0) return;
            for (int p = 0; p < inhabitants.Count; p++)
            {
                CardTMP personCard = _personListView.SpawnItem(p.ToString(), _lotCardPrefab.gameObject)
                    .GetComponent<CardTMP>();
                personCard.SetHeader(inhabitants[p].Name);
                personCard.SetLabel(inhabitants[p].ToString());
            }
            _personListView.UpdateLayout();
            _tooltipPanel.SetActive(true);
        }

        private void UpdateEmployeeTooltip()
        {
            if (_current is not Workplace workplace) return;
            List<Person> employees = workplace.GetEmployees();
            if (employees.Count == 0) return;
            for (int p = 0; p < employees.Count; p++)
            {
                CardTMP personCard = _personListView.SpawnItem(p.ToString(), _personCard.gameObject)
                    .GetComponent<CardTMP>();
                personCard.SetLabel(employees[p].Name);
            }
            _personListView.UpdateLayout();
            _tooltipPanel.SetActive(true);
        }

        private void CloseTooltip()
        {
            _currentTooltip = string.Empty;
            _tooltipPanel.SetActive(false);
        }

        private void ClockUpdate(int tick)
        {
            if (!_current) return;
            string capacity = $"<color=#{_uiManager.ColorPalette.PositiveHex}>{_current.GetPersonsCount()}</color> / {_current.GetMaxCapacity()}";
            _capacityCards.ForEach(card =>
            {
                card.SetLabel(card.ID == CardTypes.Patrons.ToString()
                    ? $"[{_current.GetPatronCriteria()}]\n{capacity}"
                    : $"{capacity}");
                if (_tooltipToggles.TryGetValue(card.ID, out Toggle toggle))
                {
                    toggle.interactable = _current.GetPersonsCount() > 0;
                }
            });
        }
    }
}