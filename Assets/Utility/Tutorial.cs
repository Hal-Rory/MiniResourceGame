using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using Placement;
using UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public List<GameObject> TutorialWindows;
    [SerializeField] private Button _menuButton;
    [SerializeField] private Image _selectionMenuButtonImage;
    [SerializeField] private GameObject _placementBorder;
    [SerializeField] private TownLotSelectionManager _townLotSelection;
    private TownLot _placedLot;
    private int _tutorialIndex;
    private string _tutorialName;
    [SerializeField] private int _lotWindow;
    public ObjectSelectionUI Selection;

    private void Start()
    {
        Assert.IsTrue(TutorialWindows is { Count: > 0 }, "Add the tutorial windows");
        GoBackToStep(0);
        GameController.Instance.TownPopulace.SetStagnant(true);
        GameController.Instance.GameTime.SetTimeActive(false);

        _menuButton.onClick.AddListener(MenuButton);
        TutorialWindows[_tutorialIndex].gameObject.SetActive(true);
        _tutorialName = TutorialWindows[_tutorialIndex].name;
    }

    private void MenuButton()
    {
        ColorBlock colors = _menuButton.colors;
        colors.normalColor = Color.white;
        _menuButton.colors = colors;
        _menuButton.onClick.RemoveListener(MenuButton);

        MoveOn();
        GameController.Instance.TownObject.OnStateChanged += MoveOnSelection;
    }

    private void MoveOnSelection(bool moveOn)
    {
        if (!moveOn) return;
        GameController.Instance.RegisterPlacementListener(PlacedLot);
        _placementBorder.SetActive(true);
        MoveOn();
    }

    private void PlacedLot(TownLot lot)
    {
        _placementBorder.SetActive(false);
        MoveOn();
        ((RectTransform)TutorialWindows[_lotWindow].transform).position =
            GameController.Instance.Input.WorldToScreen(lot.CellBlock + Vector3.up * 2);
        _placedLot = lot;
        Selection.Shutdown();
        _townLotSelection.gameObject.SetActive(true);
        GameController.Instance.GameTime.SetTimeActive(true);
        GameController.Instance.Selection.OnTownObjectSelected += OnTownObjectSelected;
    }

    private void OnTownObjectSelected(TownLot obj)
    {
        MoveOn();
        _selectionMenuButtonImage.color = Color.yellow;
        GameController.Instance.Selection.OnTownObjectSelected -= OnTownObjectSelected;
        GameController.Instance.Selection.OnTownObjectDeselected += OnTownObjectDeselected;
    }

    private void OnTownObjectDeselected(TownLot obj)
    {
        MoveOn();
        GameController.Instance.Selection.OnTownObjectDeselected -= OnTownObjectDeselected;
        StartCoroutine(WaitForClock());
    }

    private IEnumerator WaitForClock()
    {
        _menuButton.interactable = false;
        _townLotSelection.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => GameController.Instance.Input.PrimaryPressed);
        MoveOn();
        GameController.Instance.Population.CreateHome(_placedLot as House);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => GameController.Instance.Input.PrimaryPressed);
        MoveOn();
        GameController.Instance.Income.Pay(100);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => GameController.Instance.Input.PrimaryPressed);
        MoveOn();
        enabled = false;
    }

    public void MoveOn()
    {
        if (TutorialWindows[_tutorialIndex].name != _tutorialName) return;
        TutorialWindows[_tutorialIndex++].SetActive(false);
        TutorialWindows[_tutorialIndex].SetActive(true);
        _tutorialName = TutorialWindows[_tutorialIndex].name;
    }

    private void GoBackToStep(int index)
    {
        foreach (GameObject window in TutorialWindows)
        {
            window.SetActive(false);
        }

        _tutorialIndex = index;
        TutorialWindows[_tutorialIndex].SetActive(true);
        _tutorialName = TutorialWindows[_tutorialIndex].name;
    }
}