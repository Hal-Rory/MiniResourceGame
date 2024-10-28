using Town.TownPopulation;
using UnityEngine;

[CreateAssetMenu(fileName = "New Job", menuName = "Town/Create Job Object")]
public class WorkplaceLotObj : TownLotObj
{
    public int Wages;
    public PersonAgeGroup[] AgeGroups;
    public int EmployeeLimit;
    public EmploymentSpeciality JobSpeciality;
    public TownResource JobResource;
}