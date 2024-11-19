using Town.TownPopulation;
using UnityEngine;
using Utility;

[CreateAssetMenu(fileName = "New Job", menuName = "Town/Create Job Object")]
public class WorkplaceLotObj : TownLotObj
{
    public int Wages;
    public PersonAgeGroup[] AgeGroups;
    public int EmployeeLimit;
}