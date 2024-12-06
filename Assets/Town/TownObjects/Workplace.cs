using System;
using System.Collections.Generic;
using System.Linq;
using Placement;
using Town.TownPopulation;
using UnityEngine;

namespace Town.TownObjects
{
    public class Workplace : TownLot, IIncomeContributor
    {
        [SerializeField] private List<Person> _employees = new List<Person>();
        [SerializeField] private List<Person> _patrons = new List<Person>();
        private WorkplaceLotObj _workLotData => _townLotData as WorkplaceLotObj;
        public int EmployeeCount => _employees.Count;
        public int MaxEmployeeCapacity => _workLotData.EmployeeLimit;
        public int Wages => _workLotData.Wages;

        public string GetWorkCriteria()
        {
            string employeeCriteria = String.Join(", ", _workLotData.EmployeeAgeGroups.Select(age => age.ToString()).ToList());
            return employeeCriteria;
        }

        public List<Person> GetEmployees()
        {
            return _employees;
        }

        public override List<Person> GetPersons()
        {
            return _patrons;
        }

        public bool CanHire(Person person)
        {
            return _employees.Count < _workLotData.Capacity && _workLotData.EmployeeAgeGroups.Contains(person.AgeGroup);
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
    }
}