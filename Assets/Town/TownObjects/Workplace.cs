using System;
using System.Collections.Generic;
using Town.TownPopulation;
using UnityEngine;

public class Workplace : TownLot, IIncomeContributor
{
    private int _wages;
    [field: SerializeField] public int IncomeContribution { get; private set; }

    public int GetIncomeContribution()
    {
        return IncomeContribution;
    }

    [field: SerializeField] public bool CanContribute { get; private set; }
    [SerializeField] private List<Person> _employees = new List<Person>();
    [field: SerializeField] public PersonAgeGroup[] AgeGroups { get; private set; }

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

    private void Update()
    {
        CanContribute = _employees.Count != 0;
    }

    public override void Create(TownObj obj)
    {
        WorkplaceObj workplace = obj as WorkplaceObj;
        AgeGroups = workplace.AgeGroups;
        _wages = workplace.Wages;
    }
}