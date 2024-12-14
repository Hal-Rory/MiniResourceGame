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
        public void AddVisitors(params Person[] persons)
        {
            _persons.AddRange(persons);
        }

        public List<Person> GetVisitors()
        {
            return _persons;
        }

        public int GetVisitorCount()
        {
            return _persons.Count;
        }

        public Sprite GetLotDepiction()
        {
            return _townLotData.ObjPreview;
        }

        public string GetLotPerks()
        {
            return _townLotData.GetPerks();
        }

        public int GetLotPrice()
        {
            return _townLotData.LotPrice;
        }

        public int GetMaxVisitorCapacity()
        {
            return _townLotData.VisitorCapacity;
        }

        public void SetLotID(int ID)
        {
            PlacementID = ID;
        }

        public bool CheckHappinessGroup(PersonAgeGroup age)
        {
            return _townLotData.VisitorAgeTarget.Length > 0 &&
                   (_townLotData.VisitorAgeTarget.Contains(age) ||
                    _townLotData.VisitorAgeTarget.Contains(PersonAgeGroup.All));
        }

        protected void SetLotName(string lotName)
        {
            _name = lotName;
        }

        public virtual string GetLotName()
        {
            return _name;
        }

        public float GetLotHappiness()
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

        public void RemoveVisitor(params Person[] persons)
        {
            _persons.RemoveAll(persons.Contains);
        }

        public void RemoveAllVisitors()
        {
            RemoveVisitor(_persons.ToArray());
        }

        public virtual void StartHovering()
        {
        }

        public virtual void EndHovering()
        {
        }

        /// <summary>
        /// Fill in lot data from town lot object, such as name, sprite, and lot "size"
        /// </summary>
        /// <param name="lotObj"></param>
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
    }
}