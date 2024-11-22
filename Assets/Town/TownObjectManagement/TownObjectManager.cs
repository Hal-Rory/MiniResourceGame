using System;
using System.Collections.Generic;
using Common.Utility;
using Town.TownObjectData;
using UnityEngine;

[Serializable]
public class TownObjectManager : IControllable
{
    [SerializeField] private List<ObjectCollection> _objectCollections;
    private int _currentCollection;
    private int _currentObject;
    public TownLotObj CurrentObject => _objectCollections[_currentCollection].Objects.Count == 0 || _currentObject < 0 ? null : _objectCollections[_currentCollection].Objects[_currentObject];
    public event Action OnCollectionChanged;
    public event Action OnSelectionChanged;
    public event Action<bool> OnStateChanged;

    public void SetUp()
    {
        _objectCollections = new List<ObjectCollection>(Resources.LoadAll<ObjectCollection>("TownObjects"));
        _currentCollection = 0;
    }

    public void SetDown()
    {
    }

    public int GetCurrentCollection()
    {
        return _currentCollection;
    }

    public void SetObjectSelection(string id)
    {
        _currentObject = string.IsNullOrEmpty(id) ? -1 : _objectCollections[_currentCollection].Objects.FindIndex(o => o.ID == id);
        OnSelectionChanged?.Invoke();
    }

    public TownLotObj[] GetObjectsInCollection()
    {
        return _objectCollections[_currentCollection].Objects.ToArray();
    }

    public void ChangeCollection(int index)
    {
        if(!index.IsBetweenRange(0, _objectCollections.Count-1))
        {
            Debug.LogWarning($"{nameof(TownObjectManager)}: Index is out of bounds");
            return;
        }

        _currentObject = 0;
        _currentCollection = index;
        OnCollectionChanged?.Invoke();
    }

    public void StartSelection(bool started)
    {
        OnStateChanged?.Invoke(started);
    }
}