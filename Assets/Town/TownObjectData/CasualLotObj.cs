using Town.TownObjects;
using UnityEngine;

namespace Town.TownObjectData
{
    /// <summary>
    /// A lot that is not a workplace or a house
    /// </summary>
    [CreateAssetMenu(fileName = "New Casual Lot", menuName = "Town/Create Casual Lot Object")]
    public class CasualLotObj : TownLotObj
    {
        /// <summary>
        /// Can this lot be replaced without removing it first
        /// </summary>
        public bool CanBeOverwritten;

        /// <inheritdoc/>
        ///<remarks>
        /// This adds a simple casual lot
        /// </remarks>
        public override TownLot AddLotType(GameObject go)
        {
            TownLot lot = go.gameObject.AddComponent<CasualLot>();
            return lot;
        }
    }
}