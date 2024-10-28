using System;
using UnityEngine;
using UnityEngine.UIElements;

public class HousingUI : MonoBehaviour
{
    public ButtonCard Inhabitants;
    private House _target;
    private bool _hasInhabitants => _target && _target.Household != null;
    private bool _householdFound;
    private PopulationFactory _population => GameController.Instance.Population;

    public GameObject Commands;

    private void OnEnable()
    {
        _population.OnPopulationChanged += PopulationCheck;
    }

    private void OnDisable()
    {
        if(GameController.Instance) _population.OnPopulationChanged -= PopulationCheck;
    }

    private void Update()
    {
        if (!_target) return;
        UpdateInhabitantLabel();
    }

    public void PopulationCheck()
    {
        if (!_target) return;
        _householdFound = _population.HomelessMatch(_target) != null;
    }
    public void DisplayHousehold(House target)
    {
        _target = target;
        Commands.SetActive(_target);
        if (!Commands.activeSelf) return;
        PopulationCheck();
        UpdateInhabitantLabel();
    }

    public void DemolishHouse_Button()
    {
        GameController.Instance.RemoveLot(_target.CellBlock);
    }

    public void InhabitantControl_Button()
    {
        if (_target.Household != null)
        {
            _population.OrphanHousehold(_target);
        }
        else
        {
            _population.TryCreateHome(_target);
        }
    }

    private void UpdateInhabitantLabel()
    {
        switch (_hasInhabitants)
        {
            case false:
                Inhabitants.SetLabel(_householdFound ? "Move In" : "Create New");
                break;
            case true:
                Inhabitants.SetLabel("Evict");
                break;
        }
    }
}