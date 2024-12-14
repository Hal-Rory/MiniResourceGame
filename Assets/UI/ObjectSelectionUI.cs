using System.Collections.Generic;
using Controllers;
using Town.TownObjectData;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ObjectSelectionUI : MonoBehaviour, IUIControl
    {
        private InputManager _input => GameController.Instance.Input;
        private TownObjectManager _townObject => GameController.Instance.TownObject;
        public GameObject MenuContainer;
        public bool Active { get; set; }
        public ToggleCard SelectionTogglePrefab;
        public ListView SelectionDisplay;
        private Toggle _currentSelection;
        private SoundManager _soundManager => GameController.Instance.Sound;
        [SerializeField] private GameObject _lotDescription;
        [SerializeField] private ScrollRect _lotDescriptionScroll;
        [SerializeField] private List<CardTMP> _lotCards;
        [SerializeField] private CardTMP _headerCard;
        [SerializeField] private Text _currentCollection;
        [SerializeField] private Text _fundsWarning;
        [SerializeField] private GameObject _buildButton;
        [SerializeField] private ToggleGroup _selectionToggleGroup;

        private void Start()
        {
            MenuContainer.SetActive(false);
            CloseScroll();
            _townObject.OnCollectionChanged += DoOnCollectionChanged;
            SetCurrentCollection();
            GameController.Instance.Income.OnIncomeUpdated += UpdatePricing;
            GameController.Instance.TownObject.OnStateChanged += OnStateChanged;
        }

        private void OnStateChanged(bool obj)
        {
            MenuContainer.gameObject.SetActive(false);
            CloseScroll();
        }

        private void Update()
        {
            if (_input.MenuPressed)
            {
                SetMenuOpen(!Active);
            }

            if (_input.ExitPressed)
            {
                if (Active)
                {
                    SetMenuOpen(false);
                }
            }

            if (_input.SecondaryPressed)
            {
                // print(_townObject.GetObjectsInCollection()[_currentSelection].ID);
                // print(SelectionDisplay.GetCard(_townObject.GetObjectsInCollection()[_currentSelection].ID).ID);
                // ((ButtonCard)SelectionDisplay.GetCard(_townObject.GetObjectsInCollection()[_currentSelection].ID)).Select();
            }
        }

        #region Menu State Control

        public void ToggleMenu_Button()
        {
            SetMenuOpen(!Active);
        }

        private void SetMenuOpen(bool open)
        {
            if (open && !GameController.Instance.UI.TrySetActive(this)) return;
            ClearCards();
            if(!open)
            {
                GameController.Instance.UI.EndControl(this);
            }
            else
            {
                _soundManager.PlaySelect();
                SetCurrentCollection();
                ModifyList();
            }
            MenuContainer.SetActive(open);
        }

        public void ForceShutdown()
        {
            if (!GameController.Instance.UI.HasControl(this)) return;
            _townObject.StartPlacing(false);
        }

        #endregion

        #region List Display

        public void BeginPlacing()
        {
            _soundManager.PlaySelect();
            _townObject.StartPlacing(true);
        }

        private void ModifyList()
        {
            TownLotObj[] townObjs = _townObject.GetObjectsInCollection();
            foreach (TownLotObj lot in townObjs)
            {
                ToggleCard card = SelectionDisplay.SpawnItem(lot.ID, SelectionTogglePrefab.gameObject).GetComponent<ToggleCard>();
                card.Set(lot.ID);
                void onSelect(bool selected)
                {
                    if (selected)
                    {
                        _soundManager.PlaySelect();
                        _currentSelection = card.GetToggle();
                    }
                    else
                    {
                        if (!_selectionToggleGroup.AnyTogglesOn())
                        {
                            _soundManager.PlayCancel();
                        }
                    }

                    SetLotDescription(_townObject.SetObjectSelection(selected ? card.ID : string.Empty), lot);
                }
                card.AddListener(onSelect);
                card.SetIcon(lot.ObjPreview);
                card.GetToggle().group = _selectionToggleGroup;
            }

            SelectionDisplay.UpdateLayout();
        }

        private void SetLotDescription(bool active, TownLotObj lot)
        {
            if (active)
            {
                _headerCard.SetHeader($"{lot.Name}");
                _headerCard.SetLabel($"${lot.LotPrice}");
                _headerCard.SetIcon(lot.ObjPreview);
                bool canAfford = GameController.Instance.CanPurchase(_townObject.CurrentObject);
                _buildButton.gameObject.SetActive(canAfford);
                _fundsWarning.gameObject.SetActive(!canAfford);
                foreach (CardTMP card in _lotCards)
                {
                    card.gameObject.SetActive(false);
                    switch (lot)
                    {
                        case not null when card.ID == TownLotUI.CardTypes.Perks.ToString():
                            card.SetLabel($"{lot.GetPerks()}");
                            card.gameObject.SetActive(true);
                            break;
                        case not HousingLotObj when card.ID == TownLotUI.CardTypes.Visitors.ToString():
                            card.SetLabel($"[Age Groups: {lot.GetVisitorCriteria()}]{(lot.VisitorAgeTarget.Length > 0 ? $"\n{lot.VisitorCapacity}" : "")}");
                            card.gameObject.SetActive(true);
                            break;
                        case WorkplaceLotObj workplace when card.ID == TownLotUI.CardTypes.Employees.ToString():
                            card.SetLabel($"[Age Groups: {workplace.GetEmployeeCriteria()}]\n{workplace.EmployeeCapacity}");
                            card.gameObject.SetActive(true);
                            break;
                        case HousingLotObj house when card.ID == TownLotUI.CardTypes.Inhabitants.ToString():
                            card.SetLabel($"{house.InhabitantCapacity}");
                            card.gameObject.SetActive(true);
                            break;
                    }
                }
                _lotDescription.SetActive(true);
            }
            else
            {
                ClosetLotDescription();
            }
        }

        public void ClosetLotDescription()
        {
            CloseScroll();
            if (!_currentSelection) return;
            _currentSelection.isOn = false;
            _currentSelection = null;
        }

        private void CloseScroll()
        {
            _lotDescriptionScroll.verticalNormalizedPosition = 1;
            _lotDescription.SetActive(false);
        }

        private void ClearCards()
        {
            SelectionDisplay.ClearCards();
        }

        private void UpdatePricing()
        {
            if (!Active || !_lotDescription.activeSelf) return;
            bool canAfford = GameController.Instance.CanPurchase(_townObject.CurrentObject);
            _buildButton.gameObject.SetActive(canAfford);
            _fundsWarning.gameObject.SetActive(!canAfford);
        }

        #endregion

        #region Collections

        private void SetCurrentCollection()
        {
            _currentCollection.text = _townObject.GetCurrentCollectionName();
        }

        public void SetNextCollection()
        {
            _townObject.NextCollection();
        }

        public void SetPreviousCollection()
        {
            _townObject.PreviousCollection();
        }

        private void DoOnCollectionChanged()
        {
            CloseScroll();
            _soundManager.PlayPlace();
            ClearCards();
            if (Active)
            {
                ModifyList();
            }
            SetCurrentCollection();
        }
        #endregion
    }
}