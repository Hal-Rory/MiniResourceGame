using Placement;
using Town.TownObjects;
using UnityEngine;

namespace Town.TownObjectData
{
    [CreateAssetMenu(fileName = "New Casual Lot", menuName = "Town/Create Casual Lot Object")]
    public class CasualLotObj : TownLotObj
    {
        public override TownLot AddLotType(GameObject go)
        {
            TownLot lot = go.gameObject.AddComponent<CasualLot>();
            return lot;
        }
    }
}