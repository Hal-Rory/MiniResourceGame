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
    void Start()
    {
        Panel.SetActive(false);
        GameController.Instance.Selection.OnTownObjectSelected += TownLotSelected;
        GameController.Instance.Selection.OnTownObjectDeselected += TownLotDeselected;
    }

    private void Update()
    {
        if (!_current) return;
        Banner.text = _current.name;
        Description.text = _current.ToString();
        Icon.sprite = _current.GetDepiction();
        Icon.gameObject.SetActive(Icon.sprite);
    }

    private void TownLotDeselected(TownLot lot)
    {
        GameController.Instance.UI.EndControl(this);
            Panel.SetActive(false);
            _current = null;
    }

    private void TownLotSelected(TownLot lot)
    {
        if (!GameController.Instance.UI.TrySetActive(this)) return;
        Panel.SetActive(true);
        _current = lot;
    }
}