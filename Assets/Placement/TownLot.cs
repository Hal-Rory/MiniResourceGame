using System.Collections.Generic;
using System.Linq;
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
        protected TownLotObj _townLotData;
        public string LotType => _townLotData.LotType;
        [SerializeField] protected SpriteRenderer _renderer;
        [SerializeField] protected BoxCollider2D _collider;
        public abstract int GetPersonsCount();
        public abstract List<Person> GetPersons();

        private void Awake()
        {
            _renderer = transform.Find("Display").GetComponent<SpriteRenderer>();
            _collider = GetComponentInChildren<BoxCollider2D>();
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
            return _townLotData.HappinessAgeTarget.Contains(age) ||
                   _townLotData.HappinessAgeTarget.Contains(PersonAgeGroup.All);
        }

        public string GetName()
        {
            return _townLotData.Name;
        }

        public float GetHappiness()
        {
            return _townLotData.Happiness;
        }

        public string GetPatronCriteria()
        {
            return _townLotData.GetPatronCriteria();
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
            _renderer.sprite = lotObj.ObjPlacement;
            _collider.size = _renderer.bounds.size;
            _collider.offset = transform.InverseTransformPoint(_renderer.bounds.center);
        }
    }
}