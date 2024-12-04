using System;
using Common.UI;
using Controllers;
using Town.TownObjectData;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ObjectSelectionUI : MonoBehaviour, IUIControl
    {
        private TownObjectManager _townObject => GameController.Instance.TownObject;
        public GameObject Menus;
        public ToggleCard[] Tabs;
        private bool _menuOpen;
        private InputManager _input => GameController.Instance.Input;
        public int _currentSelection;
        public ButtonCard SelectionButtonPrefab;
        public CardListView SelectionDisplay;

        private void Start()
        {
            Menus.SetActive(false);
            _townObject.OnCollectionChanged += DoOnCollectionChanged;
            _townObject.OnSelectionChanged += DoSelectionChanged;
            SetCurrentTab(_townObject.GetCurrentCollection(), true);
            GameController.Instance.Income.OnIncomeUpdated += UpdatePricing;
        }

        private void Update()
        {
            print(_input.Movement);
            if (_input.MenuPressed)
            {
                SetMenuOpen(!_menuOpen);
            }

            if (_input.ExitPressed)
            {
                if (_menuOpen)
                {
                    SetMenuOpen(false);
                }
            }

            if (_input.SecondaryPressed)
            {
                print(_townObject.GetObjectsInCollection()[_currentSelection].ID);
                print(SelectionDisplay.GetCard(_townObject.GetObjectsInCollection()[_currentSelection].ID).ID);
                ((ButtonCard)SelectionDisplay.GetCard(_townObject.GetObjectsInCollection()[_currentSelection].ID)).Select();
            }
        }

        #region Menu State Control

        public void ToggleMenu_Button()
        {
            SetMenuOpen(!_menuOpen);
        }


        private void SetMenuOpen(bool open)
        {
            if (open && !GameController.Instance.UI.TrySetActive(this)) return;
            ClearCards();
            _menuOpen = open;
            if(!open)
            {
                GameController.Instance.UI.EndControl(this);
            }
            else
            {
                ModifyList();
            }
            Menus.SetActive(open);
        }

        public void Shutdown()
        {
            if (!GameController.Instance.UI.HasControl(this)) return;
            _menuOpen = false;
            _townObject.StartSelection(false);
        }

        #endregion

        #region Selection Menu

        private void DoSelectionChanged()
        {
            if (!_menuOpen) return;
            UpdateObjectSelection();
        }

        #endregion

        #region During Selection

        private void UpdateObjectSelection()
        {
            _townObject.StartSelection(true);
        }
        #endregion

        #region List Display

        private void ModifyList()
        {
            TownLotObj[] townObjs = _townObject.GetObjectsInCollection();
            foreach (TownLotObj lot in townObjs)
            {
                ButtonCard card = SelectionDisplay.SpawnItem(lot.ID, lot.Name, SelectionButtonPrefab) as ButtonCard;
                void onSelect()
                {
                    _townObject.SetObjectSelection(_townObject.CurrentObject && _townObject.CurrentObject.ID == card.ID ? null : card.ID);
                }
                card.AddListener(onSelect);
                card.SetIcon(lot.ObjPreview);
                card.Interactable = GameController.Instance.CanPurchase(lot);
            }

            SelectionDisplay.UpdateLayout();
        }

        private void ClearCards()
        {
            SelectionDisplay.ClearCards();
        }

        private void UpdatePricing()
        {
            if (!_menuOpen) return;
            TownLotObj[] cards = _townObject.GetObjectsInCollection();
            foreach (TownLotObj lot in cards)
            {
                if (!SelectionDisplay.SpawnedCards.TryGetValue(lot.ID, out Card card))
                {
                    ClearCards();
                    ModifyList();
                    return;
                }
                if (lot)
                    ((ButtonCard)card).Interactable = GameController.Instance.CanPurchase(lot);
            }
        }

        #endregion

        #region Toggles

        /// <summary>
        /// Performing Toggle groups job because Unity's toggle group is incredibly noisy
        /// </summary>
        /// <param name="index"></param>
        /// <param name="silent"></param>
        private void SetCurrentTab(int index, bool silent)
        {
            void setToggle(bool value, ToggleCard t)
            {
                if (silent)
                {
                    t.GetToggle().SetIsOnWithoutNotify(value);
                    t.SetIconActive(value);
                }
                else
                {
                    t.GetToggle().isOn = value;
                    t.SetIconActive(value);
                }
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

        public void ChangeCollection_Toggle(int selection)
        {
            if (!_menuOpen || selection == _townObject.GetCurrentCollection()) return;
            _townObject.ChangeCollection(selection);
        }

        private void DoOnCollectionChanged()
        {
            SetCurrentTab(_townObject.GetCurrentCollection(), true);
            ClearCards();
            if (_menuOpen)
            {
                ModifyList();
            }
        }

        #endregion
    }
}