using System;
using System.Collections.Generic;
using System.Linq;
using Common.Utility;
using Random = UnityEngine.Random;

namespace Town.TownPopulation
{
    [Serializable]
    public class Person : ITimeListener
    {
        private static int[] _ageRanges => new int[]
        {
            12,
            18,
            50,
            90
        };
        public string Name;
        public PersonAgeGroup AgeGroup;
        private float _age;
        public int JobIndex = -1;
        public int HouseholdIndex;
        public int IncomeContribution;
        public bool Homeless;

        private Dictionary<PersonStats, float> _stats;
        private Dictionary<int, float> _relationships;

        private EmploymentSpeciality _employmentSpeciality;
        private bool _employmentMatch;

        public int ID { get; private set; }

        public bool CanWork => !Homeless && AgeGroup != PersonAgeGroup.Deceased;

        public void Setup(int id)
        {
            int age = (int)AgeGroup;
            int ageMin = AgeGroup == PersonAgeGroup.Child ? 1 : _ageRanges[(int)AgeGroup - 1];
            _age = Random.Range(ageMin,_ageRanges[age]);
            _employmentMatch = false;
            _relationships = new Dictionary<int, float>();
            ID = id;
            _stats = PopulationUtility.StatSetup();
            _stats[PersonStats.Happiness] = 100;
            _stats[PersonStats.Health] = 100;
            _stats[PersonStats.Hunger] = 100;
        }

        public void AddRelationship(params int[] personID)
        {
            Array.ForEach(personID, p =>
            {
                if(p == ID) return;
                _relationships.Add(p, 0);
            });
        }

        public void Unemploy()
        {
            JobIndex = -1;
            IncomeContribution = 0;
            _employmentMatch = false;
        }

        public void Employ(int placementID, int wages, EmploymentSpeciality _speciality)
        {
            JobIndex = placementID;
            IncomeContribution = wages;
            _employmentMatch = _employmentSpeciality == _speciality;
        }

        public void Evict()
        {
            Unemploy();
            Homeless = true;
        }

        public float GetStatValue(PersonStats stat)
        {
            return _stats[stat];
        }

        public void ClockUpdate(int tick)
        {
            float ageFactor = tick / 100f;
            if (!IsAlive(ageFactor)) return;

            _stats[PersonStats.Happiness] += ageFactor * (_employmentMatch ? 1f : -1f);
            _stats[PersonStats.Health] += ageFactor * (_stats[PersonStats.Hunger] > 25f ? 1f : -1f);
            _stats[PersonStats.Health].Clamp(0, 100);
            _stats[PersonStats.Hunger] -= ageFactor;
            _stats[PersonStats.Hunger].Clamp(0, 100);
            float social = Random.Range(-.5f,0f);
            foreach (var relationship in _relationships.Keys.ToList())
            {
                float change = Random.Range(-1f,1f);
                _relationships[relationship] += change;
                social += change;
            }

            if(_relationships.Count > 0) _stats[PersonStats.Happiness] += social/_relationships.Count;
            _stats[PersonStats.Happiness].Clamp(0, 100);
        }

        private bool IsAlive(float ageFactor)
        {
            if (AgeGroup == PersonAgeGroup.Deceased) return false;
            if (_stats[PersonStats.Health] == 0)
            {
                AgeGroup = PersonAgeGroup.Deceased;
                Unemploy();
                return false;
            }
            _age += ageFactor;
            int age = (int)AgeGroup;
            if (_age > _ageRanges[age])
            {
                AgeGroup++;
            }

            if (AgeGroup == PersonAgeGroup.Deceased)
            {
                Unemploy();
            }

            return AgeGroup != PersonAgeGroup.Deceased;
        }

        public override string ToString()
        {
            return $"{Name} | {AgeGroup} (Income:{IncomeContribution:+0;-#})";
        }
    }
}