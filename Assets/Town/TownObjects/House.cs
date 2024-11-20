using System.Linq;
using Town.TownObjectData;
using Town.TownPopulation;
using UnityEngine;

public class House : TownLot
{
    [field: SerializeField] public Household Household { get; private set; }

    [SerializeField] private GameObject _hoverBG;
    private HousingLotObj _houseLotData => _townLotData as HousingLotObj;

    public void SetHousehold(Household household)
    {
        Household = household;
    }

    public override void StartHovering()
    {
        _hoverBG.SetActive(true);
    }

    public override void EndHovering()
    {
        _hoverBG.SetActive(false);
    }

    public override void Create(TownLotObj lotObj)
    {
        _townLotData = lotObj;
        SetDisplay();
    }

    protected override void SetDisplay()
    {
        _renderer.sprite = _townLotData.ObjPreview;
    }

    public int GetHousingSize()
    {
        return _houseLotData.HouseholdSize;
    }

    public override string ToString()
    {
        string inhabitants = string.Empty;
        if (Household != null)
            inhabitants = Household.GetInhabitants().Aggregate(
                inhabitants,
                (current, inhabitant) => current + $"\n{inhabitant}");

        return $"{_townLotData.Name}" +
               (!string.IsNullOrEmpty(inhabitants)
                   ? $"\nResidents: {inhabitants}"
                   : "\nCurrently Vacant");
    }
}