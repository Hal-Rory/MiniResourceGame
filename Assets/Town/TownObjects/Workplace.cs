using System;
using System.Collections.Generic;
using System.Linq;
using Town.TownPopulation;
using UnityEngine;
using Utility;

public class Workplace : TownLot, IIncomeContributor
{
    [SerializeField] private GameObject _hoverBG;
    [SerializeField] private List<Person> _employees = new List<Person>();
    private WorkplaceLotObj _townLotData;
    public bool CanHire(Person person)
    {
        return _employees.Count <= _townLotData.EmployeeLimit && _townLotData.AgeGroups.Contains(person.AgeGroup);
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
        person.Employ(PlacementID, _townLotData.Wages);
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
        return _employees.Count != 0 ? _townLotData.Wages * _employees.Count : 0;
    }

    public override string ToString()
    {
        string employees = string.Join("\n", _employees.Select(e => e.ToString()));
        string criteria = string.Join(", ", _townLotData.AgeGroups.Select(a => a.Plural()));

        return $"{_lotDescription}\n" +
               $"Currently hiring: {char.ToUpper(criteria[0]) + criteria[1..]}\n" +
               (!string.IsNullOrEmpty(employees)
                   ? $"Employees:\n{employees}"
                   : "No Workforce.");

    }

    public override void Create(TownLotObj lotObj)
    {
        _townLotData = lotObj as WorkplaceLotObj;
    }
}