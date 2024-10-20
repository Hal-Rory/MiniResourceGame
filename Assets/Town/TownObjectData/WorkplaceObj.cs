using Town.TownPopulation;
using UnityEngine;

[CreateAssetMenu(fileName = "New Job", menuName = "Town/Create Job Object")]
public class WorkplaceObj : TownObj
{
    public int Wages;
    public PersonAgeGroup[] AgeGroups;
}