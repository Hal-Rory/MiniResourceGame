using System;
using System.Collections.Generic;
using System.Linq;
using Town.TownObjectData;
using UnityEngine;

namespace Placement
{
    public class TownLotFactory : MonoBehaviour
    {
        [SerializeField] private List<TownLot> _placedGameObjects = new List<TownLot>();
        [SerializeField] private GameObject _lotPrefab;
        public event Action<TownLot> OnLotAdded;
        public event Action<TownLot> OnLotRemoved;

        public int PlaceObject(TownLotObj lotObjBase, Vector3Int position)
        {
            GameObject lotObject = Instantiate(_lotPrefab, position, Quaternion.identity);
            TownLot newLot = lotObjBase.AddLotType(lotObject);
            _placedGameObjects.Add(newLot);
            newLot.SetID(_placedGameObjects.Count - 1);
            newLot.Create(lotObjBase);
            newLot.CellBlock = position;
            OnLotAdded?.Invoke(newLot);
            return _placedGameObjects.Count - 1;
        }

        internal void RemoveObjectAt(int gameObjectIndex)
        {
            if (_placedGameObjects.Count <= gameObjectIndex
                || !_placedGameObjects[gameObjectIndex].gameObject)
                return;
            OnLotRemoved?.Invoke(_placedGameObjects[gameObjectIndex]);
            Destroy(_placedGameObjects[gameObjectIndex].gameObject);
            _placedGameObjects[gameObjectIndex] = null;
        }

        public TownLot GetLot(int index)
        {
            return _placedGameObjects[index];
        }
        public List<T> GetLots<T>(string type)
        {
            return _placedGameObjects.FindAll(lot => lot.GetType().ToString() == type) as List<T>;
        }
        public List<T> GetLots<T>(int[] ids)
        {
            return _placedGameObjects.FindAll(lot => lot != null && ids.Contains(lot.PlacementID)) as List<T>;
        }
    }
}