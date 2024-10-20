using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TownLot : MonoBehaviour
{
    public int PlacementID { get; private set; }
    public List<int> Connected { get; private set; }

    [SerializeField] private Collider2D _collider;


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_collider.bounds.center, Vector2.one * 1.5f);
    }

    public void ConnectLots()
    {
        Connected = new List<int>();
        RaycastHit2D[] lots = Physics2D.BoxCastAll(_collider.bounds.center, Vector2.one * 2.5f, 0, Vector2.zero, 0, 1 << LayerMask.NameToLayer("TownLot"));
        foreach (RaycastHit2D lot in lots)
        {
            //TODO: WebGL go boom
            float magnitude = (lot.transform.position - transform.position).magnitude;
            if (!lot.transform.parent.TryGetComponent(out TownLot townLot) || townLot.PlacementID == PlacementID || !Mathf.Approximately(magnitude, 1)) continue;
            Connected.Add(townLot.PlacementID);
            townLot.ConnectToLot(PlacementID);
        }
    }

    public void ConnectToLot(int id)
    {
        Connected.Add(id);
    }

    public void DisconnectFromLot(int id)
    {
        Connected.Remove(id);
    }

    public void DisconnectLots()
    {
        List<TownLot> lots = GameController.Instance.GetLots<TownLot>(Connected.ToArray());
        foreach (TownLot lot in lots)
        {
            lot.Connected.Remove(PlacementID);
            lot.DisconnectFromLot(PlacementID);
        }
    }

    public void SetID(int ID)
    {
        PlacementID = ID;
    }

    public abstract void Create(TownObj obj);
}