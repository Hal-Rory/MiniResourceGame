using Placement;
using Town.TownObjectData;
using UnityEngine;

[CreateAssetMenu(fileName = "New House", menuName = "Town/Create House Object")]
public class HousingLotObj : TownLotObj
{
    public int HouseholdSize;
    public override TownLot AddLotType(GameObject go)
    {
        TownLot lot = go.gameObject.AddComponent<House>();
        return lot;
    }
}