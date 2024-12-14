using System.Collections.Generic;
using Controllers;
using Placement;
using Town.TownObjectData;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private Dictionary<Vector3Int, PlacedLot> _placedLots;
    private TownLotFactory _placementManager => GameController.Instance.LotFactory;
    [SerializeField] private Collider2D _gridCollider;
    private void Awake()
    {
        _placedLots = new Dictionary<Vector3Int, PlacedLot>();
    }

    public void AddLot(TownLot lotObj, Vector3Int gridPosition, Vector2Int size)
    {
        List<Vector3Int> positions = CalculatePositions(gridPosition, size);
        PlacedLot lot = new PlacedLot
        {
            OccupiedPositions = positions,
            PlacedIndex = _placementManager.AddLot(lotObj)
        };
        foreach (Vector3Int pos in positions)
        {
            _placedLots.Add(pos, lot);
        }
    }

    public void AddLot(TownLotObj lotObj, Vector3Int gridPosition)
    {
        List<Vector3Int> positions = CalculatePositions(gridPosition, lotObj.LotSize);
        PlacedLot lot = new PlacedLot
        {
            OccupiedPositions = positions,
            PlacedIndex = _placementManager.PlaceObject(lotObj, gridPosition)
        };
        foreach (Vector3Int pos in positions)
        {
            _placedLots.Add(pos, lot);
        }
    }

    public bool RemoveLot(Vector3Int gridPosition)
    {
        if (!_placedLots.TryGetValue(gridPosition, out PlacedLot lot)) return false;
        List<Vector3Int> positions = lot.OccupiedPositions;
        _placementManager.RemoveObjectAt(lot.PlacedIndex);
        foreach (Vector3Int pos in positions)
        {
            _placedLots.Remove(pos);
        }
        return true;
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        if (objectSize == Vector2Int.zero)
        {
            Debug.Log("Placing object with no grid size will prohibit removal.");
        }
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, -y, 0));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObejctAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (_placedLots.ContainsKey(pos))
                return false;
            if(!_gridCollider.OverlapPoint(new Vector2(pos.x, pos.y)))
                return false;
        }
        return true;
    }
}