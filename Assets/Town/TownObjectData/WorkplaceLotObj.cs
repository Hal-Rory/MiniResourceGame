using Placement;
using Town.TownObjectData;
using Town.TownObjects;
using Town.TownPopulation;
using UnityEngine;
using Utility;

[CreateAssetMenu(fileName = "New Job", menuName = "Town/Create Job Object")]
public class WorkplaceLotObj : TownLotObj
{
    public int Wages;
    public PersonAgeGroup[] EmployeeAgeGroups;
    public int EmployeeLimit;
    public override TownLot AddLotType(GameObject go)
    {
        TownLot lot = go.gameObject.AddComponent<Workplace>();
        return lot;
    }
}