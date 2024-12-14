using System.Collections.Generic;
using System.Linq;
using Placement;
using Town.TownPopulation;
using UnityEngine;
using Utility;

namespace Town.TownObjects
{
    public class Workplace : TownLot, IIncomeContributor
    {
        [SerializeField] private List<Person> _employees = new List<Person>();
        private WorkplaceLotObj _workLotData => _townLotData as WorkplaceLotObj;
        public int EmployeeCount => _employees.Count;
        public int MaxEmployeeCapacity => _workLotData.EmployeeLimit;

        public string GetWorkCriteria()
        {
            return _workLotData.GetEmployeeCriteria();
        }

        public List<Person> GetEmployees()
        {
            return _employees;
        }

        public bool CanHire(Person person)
        {
            return _employees.Count < _workLotData.EmployeeLimit &&
                   (_workLotData.EmployeeAgeGroups.Contains(PersonAgeGroup.All) || _workLotData.EmployeeAgeGroups.Contains(person.AgeGroup));
        }

        public void Employ(Person person)
        {
            _employees.Add(person);
            person.Employ(PlacementID, _workLotData.Wages);
        }

        public void Unemploy(params Person[] persons)
        {
            foreach (Person person in persons)
            {
                if (person.JobIndex != PlacementID) continue;
                person.Unemploy();
                _employees.Remove(person);
            }
        }

        public void UnemployAll()
        {
            foreach (Person employee in _employees)
            {
                employee.Unemploy();
            }
            _employees.Clear();
        }

        public int GetIncomeContribution()
        {
            return _employees.Count != 0 ? _workLotData.Wages * _employees.Count : 0;
        }

        public override void RemovePersons(params Person[] persons)
        {
            base.RemovePersons(persons);
            Unemploy(persons);
        }
    }
}