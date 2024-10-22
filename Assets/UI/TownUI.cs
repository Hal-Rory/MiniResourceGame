using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TownUI : MonoBehaviour, IUIControl
{
    public Text Banner;
    public Text Description;
    public Image Icon;
    public GameObject Panel;
    void Start()
    {
        Panel.SetActive(false);
        GameController.Instance.Selection.OnTownObjectSelected += TownLotSelected;
        GameController.Instance.Selection.OnTownObjectDeselected += TownLotDeselected;
    }

    private void TownLotDeselected(TownLot lot)
    {
        GameController.Instance.UI.EndControl(this);
            Panel.SetActive(false);
    }

    private void TownLotSelected(TownLot lot)
    {
        if (!GameController.Instance.UI.TrySetActive(this)) return;
        Banner.text = lot.name;
        Description.text = lot.ToString();
        Icon.sprite = lot.GetDepiction();
        Icon.gameObject.SetActive(Icon.sprite);
        Panel.SetActive(true);
    }
}