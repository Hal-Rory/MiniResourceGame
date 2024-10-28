using System;
using UnityEngine;
using UnityEngine.UI;

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
        _townObject.SetObjectSelection(-1);
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
            for (int t = 0; t < townObjs.Length; t++)
            {
                ButtonCard card = SelectionDisplay.SpawnItem(t.ToString(), townObjs[t].Name, SelectionButtonPrefab) as ButtonCard;
                card.AddListener(onSelect);
                card.SetIcon(townObjs[t].ObjPreview);

                void onSelect()
                {
                    _townObject.SetObjectSelection(int.Parse(card.ID));
                }
            }
        }
        else
        {
            SelectionDisplay.ClearCards();
        }
        SelectionDisplay.UpdateLayout();
    }

    public void ChangeCollection_Toggle(int selection)
    {
        if (!_menuOpen) return;
        _townObject.ChangeCollection(selection);
    }
}