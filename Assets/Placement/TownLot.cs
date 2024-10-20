using UnityEngine;

public abstract class TownLot : MonoBehaviour
{
    public int PlacementID { get; private set; }

    public void SetID(int ID)
    {
        PlacementID = ID;
    }

    public abstract void Create(TownObj obj);
}