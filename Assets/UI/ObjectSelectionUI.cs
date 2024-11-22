using Common.UI;
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

        public Image CurrentObjectDisplay;
        public Text CurrentObjectLabel;
        public GameObject CurrentObject;

        public Toggle[] Tabs;

        private bool _menuOpen;
        private bool _duringSelection;
        public GameObject SelectionMenuDisplay;
        public ButtonCard SelectionButtonPrefab;
        public CardListView SelectionDisplay;

        private void Start()
        {
            SetOpenMenuDisplay(false);
            SetOpenCurrentObject(false);
            _townObject.OnCollectionChanged += DoOnCollectionChanged;
            _townObject.OnSelectionChanged += DoSelectionChanged;
            SetCurrentTab(_townObject.GetCurrentCollection(), true);
            GameController.Instance.Income.OnIncomeUpdated += UpdatePricing;
        }

        private void Update()
        {
            if (_input.ExitPressed)
            {
                OnExiting();
            }
        }

        private void OnExiting()
        {
            if (!_menuOpen) return;
            ToggleMenu_Button();
        }

        /// <summary>
        /// Performing Toggle groups job because Unity's toggle group is incredibly noisy
        /// </summary>
        /// <param name="index"></param>
        /// <param name="silent"></param>
        private void SetCurrentTab(int index, bool silent)
        {
            void setToggle(bool value, Toggle t)
            {
                if (silent)
                    t.SetIsOnWithoutNotify(value);
                else
                    t.isOn = value;
            }

            for (int i = 0; i < Tabs.Length; i++)
            {
                if (i == index)
                {
                    setToggle(true, Tabs[i]);
                    continue;
                }

                setToggle(false, Tabs[i]);
            }
        }

        private void ReturnToMenu()
        {
            if (!_menuOpen) return;
            _duringSelection = false;
            SetOpenMenuDisplay(true);
            SetOpenCurrentObject(false);
            _townObject.StartSelection(false);
        }

        private void DoSelectionChanged()
        {
            if (!_townObject.CurrentObject)
            {
                SetOpenMenuDisplay(false);
                SetOpenCurrentObject(false);
                _duringSelection = true;
                return;
            }

            if (!_menuOpen) return;
            _duringSelection = true;
            UpdateObjectSelection();
            SetOpenMenuDisplay(false);
            SetOpenCurrentObject(true);
        }

        private void DoOnCollectionChanged()
        {
            SetCurrentTab(_townObject.GetCurrentCollection(), true);
            ModifyList(false);
            ModifyList(_menuOpen);
        }

        private void UpdateObjectSelection()
        {
            _townObject.StartSelection(true);
            CurrentObjectDisplay.sprite = _townObject.CurrentObject ? _townObject.CurrentObject.ObjPreview : null;
            CurrentObjectLabel.text = _townObject.CurrentObject ? _townObject.CurrentObject.Name : string.Empty;
        }

        public void RemoveState_Button()
        {
            _townObject.SetObjectSelection(null);
            _townObject.StartSelection(true);
        }

        public void ToggleMenu_Button()
        {
            if (_duringSelection)
            {
                ReturnToMenu();
            }
            else
            {
                SetMenuOpen(!_menuOpen);
            }
        }

        private void SetOpenCurrentObject(bool open)
        {
            CurrentObject.SetActive(_townObject.CurrentObject && open);
        }

        private void SetMenuOpen(bool open)
        {
            if (!_menuOpen && !GameController.Instance.UI.TrySetActive(this)) return;
            if (!open) GameController.Instance.UI.EndControl(this);
            _menuOpen = open;
            SetOpenCurrentObject(false);
            SetOpenMenuDisplay(open);
            ModifyList(_menuOpen);
        }

        private void SetOpenMenuDisplay(bool open)
        {
            SelectionMenuDisplay.SetActive(open);
        }

        private void ModifyList(bool fill)
        {
            if (fill)
            {
                TownLotObj[] townObjs = _townObject.GetObjectsInCollection();
                foreach (TownLotObj lot in townObjs)
                {
                    ButtonCard card = SelectionDisplay.SpawnItem(lot.ID, lot.Name, SelectionButtonPrefab) as ButtonCard;
                    card.AddListener(onSelect);
                    card.SetIcon(lot.ObjPreview);
                    card.Interactable = GameController.Instance.CanPurchase(lot);

                    void onSelect()
                    {
                        _townObject.SetObjectSelection(card.ID);
                    }
                }
            }
            else
            {
                SelectionDisplay.ClearCards();
            }
            SelectionDisplay.UpdateLayout();
        }

        private void UpdatePricing()
        {
            if (!_menuOpen) return;
            TownLotObj[] cards = _townObject.GetObjectsInCollection();
            foreach (TownLotObj lot in cards)
            {
                if (!SelectionDisplay.SpawnedCards.TryGetValue(lot.ID, out Card card))
                {
                    ModifyList(false);
                    ModifyList(true);
                    return;
                }
                if (lot)
                    ((ButtonCard)card).Interactable = GameController.Instance.CanPurchase(lot);
            }
        }

        public void ChangeCollection_Toggle(int selection)
        {
            if (!_menuOpen) return;
            _townObject.ChangeCollection(selection);
        }
    }
}