using Placement;
using Town.TownObjectData;
using Town.TownObjects;
using UnityEngine;
using Utility;

[CreateAssetMenu(fileName = "New Job", menuName = "Town/Create Job Object")]
public class WorkplaceLotObj : TownLotObj
{
    public int Wages;
    public PersonAgeGroup[] EmployeeAgeGroups;
    public int EmployeeCapacity;

    /// <inheritdoc/>
    ///<remarks>
    /// This adds a workplace lot
    /// </remarks>
    public override TownLot AddLotType(GameObject go)
    {
        TownLot lot = go.gameObject.AddComponent<Workplace>();
        return lot;
    }

    /// <summary>
    /// Get the employee perks
    /// </summary>
    /// <returns></returns>
    public string GetWorkplacePerks()
    {
        return $"+${Wages} / d";
    }

    public string GetEmployeeCriteria()
    {
        if (EmployeeAgeGroups == null || EmployeeAgeGroups.Length == 0)
            return "None";
        if (EmployeeAgeGroups[0] == PersonAgeGroup.All)
        {
            return $"{PersonAgeGroup.All} ages";
        }

        string employeeCriteria = EmployeeAgeGroups.GroupedString(true, true);
        return employeeCriteria;
    }
}