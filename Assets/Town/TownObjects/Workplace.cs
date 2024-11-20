using System.Collections.Generic;
using System.Linq;
using Town.TownObjectData;
using Town.TownPopulation;
using UnityEngine;
using Utility;

public class Workplace : TownLot, IIncomeContributor
{
    [SerializeField] private GameObject _hoverBG;
    [SerializeField] private List<Person> _employees = new List<Person>();
    private WorkplaceLotObj _workLotData => _townLotData as WorkplaceLotObj;
    public bool CanHire(Person person)
    {
        return _employees.Count <= _workLotData.EmployeeLimit && _workLotData.EmployeeAgeGroups.Contains(person.AgeGroup);
    }

    public override void StartHovering()
    {
        _hoverBG.SetActive(true);
    }

    public override void EndHovering()
    {
        _hoverBG.SetActive(false);
    }

    public void Employ(Person person)
    {
        _employees.Add(person);
        person.Employ(PlacementID, _workLotData.Wages);
    }

    public void Unemploy(Person person)
    {
        _employees.Remove(person);
        person.Unemploy();
    }

    public void UnemployAll()
    {
        foreach (Person person in _employees)
        {
            person.Unemploy();
        }
        _employees.Clear();
    }

    public int GetIncomeContribution()
    {
        return _employees.Count != 0 ? _workLotData.Wages * _employees.Count : 0;
    }

    public override string ToString()
    {
        string employees = string.Join("\n", _employees.Select(e => e.ToString()));
        string criteria = string.Join(", ", _workLotData.EmployeeAgeGroups.Select(a => a.Plural()));

        return $"{_workLotData.Name}\n" +
               $"Currently hiring: {char.ToUpper(criteria[0]) + criteria[1..]}\n" +
               (!string.IsNullOrEmpty(employees)
                   ? $"Employees:\n{employees}"
                   : "No Workforce.");

    }

    public override void Create(TownLotObj lotObj)
    {
        _townLotData = lotObj as WorkplaceLotObj;
        SetDisplay();
    }

    protected override void SetDisplay()
    {
        _renderer.sprite = _townLotData.ObjPreview;
    }
}