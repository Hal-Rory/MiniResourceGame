using System;
using Controllers;
using Placement;
using UnityEngine;
using UnityEngine.UI;

public class TownLotUI : MonoBehaviour, IUIControl
{
    public Text Banner;
    public Text Description;
    public Image Icon;
    public GameObject Panel;
    private TownLot _current;

    public GameObject DemolishButton;
    void Start()
    {
        Panel.SetActive(false);
        GameController.Instance.Selection.OnTownObjectSelected += TownLotSelected;
        GameController.Instance.Selection.OnTownObjectDeselected += TownLotDeselected;
        GameController.Instance.RegisterPlacementListener(null, OnRemoveLot);
    }
    private void Update()
         {
             if (!_current) return;
             SetDisplay();
         }
    private void OnRemoveLot(TownLot obj)
    {
        if (_current != obj) return;
        TownLotDeselected(null);
    }

    public void Shutdown()
    {
        Panel.SetActive(false);
        _current = null;
        DemolishButton.gameObject.SetActive(false);
    }

    private void SetDisplay()
    {
        Banner.text = _current.name;
        Description.text = _current.ToString();
        Icon.sprite = _current.GetDepiction();
        Icon.gameObject.SetActive(Icon.sprite);
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
        _current = lot;
        DemolishButton.gameObject.SetActive(true);
        SetDisplay();
        Panel.SetActive(true);
    }

    public void DemolishLot_Button()
    {
        GameController.Instance.RemoveLot(_current.CellBlock);
    }
}