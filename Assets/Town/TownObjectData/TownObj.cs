using UnityEngine;

[CreateAssetMenu(fileName = "Town Object", menuName = "Town/Create Town Object")]
public class TownObj : ScriptableObject
{
    public string Name;
    public Vector2Int LotSize;
    public TownLot ObjLot;
    public Sprite ObjPreview;
}