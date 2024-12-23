using Town.TownObjects;
using UnityEngine;

namespace Town.TownObjectData
{
    /// <summary>
    /// A lot meant to store persons
    /// </summary>
    [CreateAssetMenu(fileName = "New House", menuName = "Town/Create House Object")]
    public class HousingLotObj : TownLotObj
    {
        public int InhabitantCapacity;

        /// <inheritdoc/>
        ///<remarks>
        /// This adds a housing lot
        /// </remarks>
        public override TownLot AddLotType(GameObject go)
        {
            TownLot lot = go.gameObject.AddComponent<House>();
            return lot;
        }
    }
}