using System;
using System.Collections.Generic;
using System.Linq;
using Town.TownPopulation;
using UnityEngine;

[Serializable]
public class Household
{
    [SerializeField] private List<Person> _inhabitants = new List<Person>();
    public int MemberCount => _inhabitants.Count;
    public int HouseID { get; private set; }
    public bool Homeless => HouseID == -1;

    public void SetHouseID(int id)
    {
        HouseID = id;
        foreach (Person inhabitant in _inhabitants)
        {
            inhabitant.Homeless = Homeless;
        }
    }

    public void AddInhabitant(Person person)
    {
        _inhabitants.Add(person);
    }

    public Person[] GetInhabitants()
    {
        return _inhabitants.ToArray();
    }

    public void FinalizeHousehold()
    {
        foreach (Person inhabitant in _inhabitants)
        {
            inhabitant.AddRelationship(_inhabitants.Select(i => i.ID).ToArray());
        }
    }

    public int GetHouseholdIncome()
    {
        return _inhabitants.Sum(person => person.IncomeContribution);
    }
}