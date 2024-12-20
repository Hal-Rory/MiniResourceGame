using System.Collections.Generic;
using System.Linq;
using TMPro;
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
        [SerializeField] protected SpriteRenderer _popupIcon;
        [SerializeField] protected TextMeshPro _popupText;
        [SerializeField] protected Animator _popupAnimator;
        private string _name;
        [SerializeField] protected List<Person> _persons = new List<Person>();
        public bool Selected { get; protected set; }
        public bool ValidLot;
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

        public void StartHovering()
        {
            _hoverObject.gameObject.SetActive(true);
        }

        public void EndHovering()
        {
            if(!Selected) _hoverObject.gameObject.SetActive(false);
        }

        public void Select()
        {
            Selected = true;
            StartHovering();
        }

        public void Deselect()
        {
            Selected = false;
            EndHovering();
        }

        public void Popup(string text, Sprite icon)
        {
            _popupText.text = text;
            _popupIcon.sprite = icon;
            _popupAnimator.SetTrigger("popup");
        }

        /// <summary>
        /// Fill in lot data from town lot object, such as name, sprite, and lot "size"
        /// </summary>
        /// <param name="lotObj"></param>
        public void Create(TownLotObj lotObj)
        {
            _townLotData = lotObj;
            _name = _townLotData.Name;

            TownLotCase lotCase = GetComponent<TownLotCase>();

            _renderer = lotCase.Renderer;
            _collider = lotCase.Collider;
            _hoverObject = lotCase.HoverObject;

            Vector3 placementPosition = new Vector3(
                lotObj.LotSize.x * .5f,
                lotObj.LotSize.y * -.5f,
                0);

            _renderer.sprite = lotObj.ObjPlacement;
            _renderer.transform.localPosition = placementPosition;
            _collider.size = lotObj.LotSize;
            _hoverObject.size = lotObj.LotSize + new Vector2(.25f, .25f);
            _hoverObject.transform.localPosition = placementPosition;

            _popupAnimator = lotCase.PopupAnimator;
            _popupIcon = lotCase.PopupIcon;
            _popupText = lotCase.PopupText;
        }
    }
}