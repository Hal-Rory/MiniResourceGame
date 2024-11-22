using Placement;
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
        public float Happiness;
        public PersonAgeGroup[] HappinessAgeTarget;

        public virtual TownLot AddLotType(GameObject go)
        {
            return null;
        }
    }
}