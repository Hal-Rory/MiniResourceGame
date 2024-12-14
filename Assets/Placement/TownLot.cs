using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Interfaces;
using Town.TownObjectData;
using Town.TownPopulation;
using UnityEngine;
using Utility;

namespace Placement
{
    public abstract class TownLot : MonoBehaviour
    {
        public Vector3Int CellBlock;
        public int PlacementID { get; private set; }
        [SerializeField] protected TownLotObj _townLotData;
        public string LotType => _townLotData.LotType;
        [SerializeField] protected SpriteRenderer _renderer;
        [SerializeField] protected BoxCollider2D _collider;
        private string _name;
        [SerializeField] protected List<Person> _persons = new List<Person>();
        private PopulationFactory _population => GameController.Instance.Population;

        private void Start()
        {
            _population.OnPopulationRemoved += OnPopulationRemoved;
        }

        private void OnDestroy()
        {
            if(GameController.Instance?.Population != null) _population.OnPopulationRemoved -= OnPopulationRemoved;
        }

        public List<Person> GetPersons()
        {
            return _persons;
        }

        public int GetPersonsCount()
        {
            return _persons.Count;
        }

        public Sprite GetDepiction()
        {
            return _townLotData.ObjPreview;
        }

        public string GetPerks()
        {
            return _townLotData.GetPerks();
        }

        public int GetPrice()
        {
            return _townLotData.LotPrice;
        }

        public int GetMaxCapacity()
        {
            return _townLotData.Capacity;
        }

        public void SetID(int ID)
        {
            PlacementID = ID;
        }

        public bool TryGetHappiness(PersonAgeGroup age)
        {
            return _townLotData.VisitorAgeTarget.Length > 0 &&
                   (_townLotData.VisitorAgeTarget.Contains(age) ||
                    _townLotData.VisitorAgeTarget.Contains(PersonAgeGroup.All));
        }

        public void SetName(string lotName)
        {
            _name = lotName;
        }

        public virtual string GetName()
        {
            return _name;
        }

        public float GetHappiness()
        {
            return _townLotData.Happiness;
        }

        public string GetVisitorCriteria()
        {
            return _townLotData.GetVisitorCriteria();
        }

        public bool CanHaveVisitors()
        {
            return _townLotData.VisitorAgeTarget.Length > 0;
        }

        public virtual void RemovePersons(params Person[] persons)
        {
            _persons.RemoveAll(persons.Contains);
        }

        public virtual void StartHovering()
        {
        }

        public virtual void EndHovering()
        {
        }

        public void Create(TownLotObj lotObj)
        {
            _townLotData = lotObj;
            _name = _townLotData.Name;
            _renderer = transform.Find("Display").GetComponent<SpriteRenderer>();
            _collider = GetComponentInChildren<BoxCollider2D>();
            _renderer.sprite = lotObj.ObjPlacement;
            _collider.size = _renderer.bounds.size;
            _collider.offset = transform.InverseTransformPoint(_renderer.bounds.center);
        }

        private void OnPopulationRemoved(IPopulation population)
        {
            switch (population)
            {
                case Person person:
                    RemovePersons(person);
                    break;
                case Household household:
                {
                    RemovePersons(household.GetInhabitants());
                    break;
                }
            }
        }
    }
}