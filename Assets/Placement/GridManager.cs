using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private Dictionary<Vector3Int, PlacedLot> _placedLots;
    private ObjectPlacer _placementManager => GameController.Instance.ObjectPlacer;
    private void Awake()
    {
        _placedLots = new Dictionary<Vector3Int, PlacedLot>();
    }

    public bool AddLot(TownObj obj, Vector3Int gridPosition)
    {
        List<Vector3Int> positions = CalculatePositions(gridPosition, obj.LotSize);
        PlacedLot lot = new PlacedLot();
        foreach (Vector3Int pos in positions)
        {
            if (!IsPositionEmpty(pos)) return false;
            _placedLots.Add(pos, lot);
        }
        lot.OccupiedPositions = positions;
        lot.PlacedIndex = _placementManager.PlaceObject(obj, gridPosition);
        return true;
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

    public bool IsPositionEmpty(Vector3Int position)
    {
        return _placedLots.Values.All(lot => !lot.OccupiedPositions.Contains(position));
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        if (objectSize == Vector2Int.zero)
        {
            Debug.LogException(new Exception("Placing object with no grid size will prohibit removal."));
        }
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }
}