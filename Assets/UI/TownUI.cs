using System;
using UnityEngine;
using UnityEngine.UI;

public class TownUI : MonoBehaviour, IUIControl
{
    public Text Banner;
    public Text Description;
    public Image Icon;
    public GameObject Panel;
    private TownLot _current;

    public HousingUI Housing;
    void Start()
    {
        Panel.SetActive(false);
        GameController.Instance.Selection.OnTownObjectSelected += TownLotSelected;
        GameController.Instance.Selection.OnTownObjectDeselected += TownLotDeselected;
        GameController.Instance.RegisterPlacementListener(null, OnRemoveLot);
    }
    private void OnRemoveLot(TownLot obj)
    {
        if (_current != obj) return;
        TownLotDeselected(null);
    }

    private void Update()
    {
        if (!_current) return;
        SetDisplay();
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
        GameController.Instance.UI.EndControl(this);
            Panel.SetActive(false);
            _current = null;
    }

    private void TownLotSelected(TownLot lot)
    {
        if (lot == null) return;
        if (!GameController.Instance.UI.TrySetActive(this)) return;
        _current = lot;
        if (lot is House house)
        {
            Housing.DisplayHousehold(house);
            Housing.gameObject.SetActive(true);
        }
        else
        {
            Housing.gameObject.SetActive(false);
        }
        SetDisplay();
        Panel.SetActive(true);
    }
}