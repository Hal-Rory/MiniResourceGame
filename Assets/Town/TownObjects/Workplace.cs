using System;
using System.Collections.Generic;
using System.Linq;
using Town.TownPopulation;
using UnityEngine;

public class Workplace : TownLot, IIncomeContributor
{
    private int _wages;
    [SerializeField] private GameObject _hoverBG;
    private int _employeeLimit;
    [SerializeField] private List<Person> _employees = new List<Person>();
    [field: SerializeField] public PersonAgeGroup[] AgeGroups { get; private set; }

    public bool CanHire(Person person)
    {
        return _employees.Count <= _employeeLimit && AgeGroups.Contains(person.AgeGroup);
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
        person.Employ(PlacementID, _wages);
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

    public Person[] GetEmployees()
    {
        return _employees.ToArray();
    }

    public int GetIncomeContribution()
    {
        return _employees.Count != 0 ? _wages * _employees.Count : 0;
    }

    public override string ToString()
    {
        string employees = string.Join("\n", _employees.Select(e => e.ToString()));
        string criteria = string.Join(", ", AgeGroups.Select(a => a.Plural()));

        return $"{_lotDescription}\n" +
               $"Currently hiring: {char.ToUpper(criteria[0]) + criteria[1..]}\n" +
               (!string.IsNullOrEmpty(employees)
                   ? $"Employees:\n{employees}"
                   : "No Workforce.");

    }

    public override void Create(TownLotObj lotObj)
    {
        WorkplaceLotObj workplaceLot = lotObj as WorkplaceLotObj;
        AgeGroups = workplaceLot.AgeGroups;
        _wages = workplaceLot.Wages;
        _employeeLimit = workplaceLot.EmployeeLimit;
        _lotDescription = lotObj.Name;
        _lotDepiction = lotObj.ObjPreview;
    }
}