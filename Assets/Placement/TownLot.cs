using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TownLot : MonoBehaviour
{
    public Vector3Int CellBlock;
    public int PlacementID { get; private set; }

    [SerializeField] private Collider2D _collider;

    protected string _lotDescription;
    protected Sprite _lotDepiction;

    public Sprite GetDepiction()
    {
        return _lotDepiction;
    }

    public void SetID(int ID)
    {
        PlacementID = ID;
    }

    public abstract void StartHovering();
    public abstract void EndHovering();

    public abstract void Create(TownLotObj lotObj);
}