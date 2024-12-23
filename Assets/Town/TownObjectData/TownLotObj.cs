using Placement;
using Town.TownObjects;
using UnityEngine;
using Utility;

namespace Town.TownObjectData
{
    [CreateAssetMenu(fileName = "Town Object", menuName = "Town/Create Town Object")]
    public class TownLotObj : ScriptableObject
    {
        public string Name;
        public string ID;
        public Vector2Int LotSize;
        public int LotPrice;
        public Sprite ObjPreview;
        public Sprite ObjPlacement;
        [Range(0,10)]
        public int Happiness;
        public string LotType;

        /// <summary>
        /// The intended visitor age groups
        /// </summary>
        public PersonAgeGroup[] VisitorAgeTarget;

        /// <summary>
        /// How many Persons can fill this lot.
        /// Each lot may use this differently
        /// </summary>
        public int VisitorCapacity;

        /// <summary>
        /// This populates the various states on a gameobject and returns the townlot component the lot object would create
        /// This is null for a townlot without a type
        /// </summary>
        /// <param name="go">The gameobject that will be getting the town lot component</param>
        /// <returns></returns>
        public virtual TownLot AddLotType(GameObject go)
        {
            return null;
        }

        public bool CanHaveVisitors()
        {
            return VisitorAgeTarget.Length > 0;
        }

        public string GetVisitorCriteria()
        {
            if (VisitorAgeTarget == null || VisitorAgeTarget.Length == 0)
                return "None";
            if (VisitorAgeTarget[0] == PersonAgeGroup.All)
            {
                return $"{PersonAgeGroup.All} ages";
            }

            string visitorCriteria = VisitorAgeTarget.GroupedString(true, true);
            return visitorCriteria;
        }

        /// <summary>
        /// Get the perks for visitor happiness
        /// </summary>
        /// <returns></returns>
        public string GetPerks()
        {
            string happinessPerk = $"{Happiness} mood / visit";
            return happinessPerk;
        }
    }
}