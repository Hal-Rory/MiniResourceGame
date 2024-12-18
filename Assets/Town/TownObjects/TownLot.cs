using System.Collections.Generic;
using System.Linq;
using Town.TownObjectData;
using Town.TownPopulation;
using UnityEngine;
using Utility;

namespace Town.TownObjects
{
    public abstract class TownLot : MonoBehaviour
    {
        public Vector3Int CellBlock;
        public int PlacementID { get; private set; }
        [SerializeField] protected TownLotObj _townLotData;
        public string LotType => _townLotData.LotType;
        [SerializeField] protected SpriteRenderer _renderer;
        [SerializeField] protected BoxCollider2D _collider;
        [SerializeField] protected SpriteRenderer _hoverObject;
        private string _name;
        [SerializeField] protected List<Person> _persons = new List<Person>();
        public bool Selected { get; protected set; }
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
            _hoverObject.gameObject.SetActive(true);
        }

        public virtual void EndHovering()
        {
            _hoverObject.gameObject.SetActive(false);
        }

        public virtual void Select()
        {
            Selected = true;
        }

        public virtual void Deselect()
        {
            Selected = false;
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
            _hoverObject = transform.Find("outline").GetComponent<SpriteRenderer>();

            Vector3 placementPosition = new Vector3(
                lotObj.LotSize.x * .5f,
                lotObj.LotSize.y * -.5f,
                0);

            _renderer.sprite = lotObj.ObjPlacement;
            _renderer.transform.localPosition = placementPosition;
            _collider.size = lotObj.LotSize;
            _hoverObject.size = lotObj.LotSize;
            _hoverObject.transform.localPosition = placementPosition;
        }
    }
}