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

    public override string GetPerks()
    {
        return $"${Wages} / mo\n{Happiness} mood / d";
    }

    public string GetEmployeeCriteria()
    {
        if (EmployeeAgeGroups == null || EmployeeAgeGroups.Length == 0)
            return "None";
        if (EmployeeAgeGroups[0] == PersonAgeGroup.All)
        {
            return $"{PersonAgeGroup.All} ages";
        }

        string employeeCriteria = EmployeeAgeGroups.GroupedString(true);
        return employeeCriteria;
    }
}