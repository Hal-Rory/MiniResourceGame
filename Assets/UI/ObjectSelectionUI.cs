using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectionUI : MonoBehaviour
{
    private TownObjectManager _townObjectManager => GameController.Instance.TownObjectManager;

    public Image CurrentObjectDisplay;
    public Text CurrentObjectLabel;
    public GameObject CurrentObject;

    private bool _menuOpen;
    private bool _duringSelection;
    public GameObject SelectionMenuDisplay;
    public ButtonCard SelectionButtonPrefab;
    public CardListView SelectionDisplay;

    private void Start()
    {
        SetMenuOpen(false);
        SetOpenCurrentObject(false);
        GameController.Instance.InputManager.OnExit += ReturnToMenu;
        _townObjectManager.OnCollectionChanged += DoOnCollectionChanged;
        _townObjectManager.OnSelectionChanged += DoSelectionChanged;
    }

    private void ReturnToMenu()
    {
        if (!_menuOpen) return;
        _duringSelection = false;
        SetOpenMenuDisplay(true);
        SetOpenCurrentObject(false);
        _townObjectManager.StartSelection(false);
    }

    private void DoSelectionChanged()
    {
        if (!_townObjectManager.CurrentObject)
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
        ModifyList(false);
        ModifyList(_menuOpen);
    }

    private void UpdateObjectSelection()
    {
        _townObjectManager.StartSelection(true);
        CurrentObjectDisplay.sprite = _townObjectManager.CurrentObject ? _townObjectManager.CurrentObject.ObjPreview : null;
        CurrentObjectLabel.text = _townObjectManager.CurrentObject ? _townObjectManager.CurrentObject.Name : string.Empty;
    }

    public void RemoveState_Button()
    {
        _townObjectManager.SetObjectSelection(-1);
        _townObjectManager.StartSelection(true);
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
        CurrentObject.SetActive(_townObjectManager.CurrentObject && open);
    }

    private void SetMenuOpen(bool open)
    {
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
            TownObj[] townObjs = _townObjectManager.GetObjectsInCollection();
            for (int t = 0; t < townObjs.Length; t++)
            {
                ButtonCard card = SelectionDisplay.SpawnItem(t.ToString(), townObjs[t].Name, SelectionButtonPrefab) as ButtonCard;
                card.AddListener(onSelect);
                void onSelect()
                {
                    _townObjectManager.SetObjectSelection(int.Parse(card.ID));
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
        _townObjectManager.ChangeCollection(selection);
    }
}