using Town.TownPopulation;
using UnityEngine;
using Utility;

[CreateAssetMenu(fileName = "Town Object", menuName = "Town/Create Town Object")]
public class TownLotObj : ScriptableObject
{
    public string Name;
    public Vector2Int LotSize;
    public int LotPrice;
    public TownLot ObjLot;
    public Sprite ObjPreview;

    public float Contentment;
    public PersonAgeGroup[] AllAges;
}