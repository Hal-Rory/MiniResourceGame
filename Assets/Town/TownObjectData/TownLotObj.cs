using UnityEngine;

[CreateAssetMenu(fileName = "Town Object", menuName = "Town/Create Town Object")]
public class TownLotObj : ScriptableObject
{
    public string Name;
    public Vector2Int LotSize;
    public int LotPrice;
    public TownLot ObjLot;
    public Sprite ObjPreview;
}