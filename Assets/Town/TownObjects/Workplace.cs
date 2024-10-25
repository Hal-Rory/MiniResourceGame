using System;
using System.Collections.Generic;
using System.Linq;
using Town.TownPopulation;
using UnityEngine;

public class Workplace : TownLot, IIncomeContributor
{
    private int _wages;
    [field: SerializeField] public int IncomeContribution { get; private set; }
    [SerializeField] private GameObject _hoverBG;
    private int _employeeLimit;
    [field: SerializeField] public bool CanContribute { get; private set; }
    [SerializeField] private List<Person> _employees = new List<Person>();
    [field: SerializeField] public PersonAgeGroup[] AgeGroups { get; private set; }

    private EmploymentSpeciality _jobSpeciality;

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

    public int GetIncomeContribution()
    {
        return IncomeContribution;
    }

    public void Employ(Person person)
    {
        _employees.Add(person);
        person.Employ(PlacementID, _wages, _jobSpeciality);
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

    private void Update()
    {
        CanContribute = _employees.Count != 0;
    }

    public override string ToString()
    {
        string employees = string.Join("\n", _employees.Select(e => e.ToString()));
        string criteria = string.Join(", ", AgeGroups.Select(a => a.Plural()));

        return $"{_lotDescription} (Upkeep:{IncomeContribution:+0;-#})\n" +
               $"Currently hiring: {char.ToUpper(criteria[0]) + criteria.Substring(1)}\n" +
               (!string.IsNullOrEmpty(employees)
                   ? $"Employees:\n" +
                     $"{employees}"
                   : "No Workforce.");

    }

    public override void Create(TownLotObj lotObj)
    {
        WorkplaceLotObj workplaceLot = lotObj as WorkplaceLotObj;
        AgeGroups = workplaceLot.AgeGroups;
        _wages = workplaceLot.Wages;
        _employeeLimit = workplaceLot.EmployeeLimit;
        _jobSpeciality = workplaceLot.JobSpeciality;
        _lotDescription = lotObj.Name;
        _lotDepiction = lotObj.ObjPreview;
    }
}