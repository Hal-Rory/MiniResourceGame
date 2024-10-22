using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TownUI : MonoBehaviour
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
        Panel.SetActive(false);
    }

    private void TownLotSelected(TownLot lot)
    {
        Banner.text = lot.name;
        Description.text = lot.ToString();
        Icon.sprite = lot.GetDepiction();
        Icon.gameObject.SetActive(Icon.sprite);
        Panel.SetActive(true);
    }
}