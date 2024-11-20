using System;
using System.Collections.Generic;
using System.Linq;
using Town.TownObjectData;
using UnityEngine;
using Utility;

public abstract class TownLot : MonoBehaviour
{
    public Vector3Int CellBlock;
    public int PlacementID { get; private set; }
    protected TownLotObj _townLotData;
    [SerializeField] protected SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = transform.Find("Display").GetComponent<SpriteRenderer>();
    }

    public Sprite GetDepiction()
    {
        return _townLotData.ObjPreview;
    }

    public int GetPrice()
    {
        return _townLotData.LotPrice;
    }

    public void SetID(int ID)
    {
        PlacementID = ID;
    }

    public bool TryGetHappiness(PersonAgeGroup age)
    {
        return _townLotData.HappinessAgeTarget.Contains(age) ||
               _townLotData.HappinessAgeTarget.Contains(PersonAgeGroup.All);
    }

    public string GetName()
    {
        return _townLotData.Name;
    }

    public float GetHappiness()
    {
        return _townLotData.Happiness;
    }

    public abstract void StartHovering();
    public abstract void EndHovering();
    public abstract void Create(TownLotObj lotObj);
    protected abstract void SetDisplay();
}