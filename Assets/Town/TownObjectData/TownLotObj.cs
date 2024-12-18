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
        [Range(0,1)]
        public float Happiness;
        public PersonAgeGroup[] VisitorAgeTarget;
        public string LotType;
        /// <summary>
        /// How many Persons can fill this lot.
        /// Each lot may use this differently
        /// </summary>
        public int VisitorCapacity;

        public virtual TownLot AddLotType(GameObject go)
        {
            return null;
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

        public virtual string GetPerks()
        {
            string happinessPerk = $"{Happiness} mood / d";
            return happinessPerk;
        }
    }
}