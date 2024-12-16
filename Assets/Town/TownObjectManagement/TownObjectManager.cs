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
    public event Action<bool> OnStateChanged;

    public void SetUp()
    {
        _objectCollections = new List<ObjectCollection>(Resources.LoadAll<ObjectCollection>("TownObjects"));
        _currentCollection = 0;
    }

    public void SetDown()
    {
    }

    public string GetCurrentCollectionName()
    {
        return _objectCollections[_currentCollection].Name;
    }

    public void NextCollection()
    {
        ChangeCollection(1);
    }

    public void PreviousCollection()
    {
        ChangeCollection(-1);
    }

    public bool SetObjectSelection(string id)
    {
        _currentObject = string.IsNullOrEmpty(id) ? -1 : _objectCollections[_currentCollection].Objects.FindIndex(o => o.ID == id);
        return _currentObject >= 0;
    }

    public TownLotObj[] GetObjectsInCollection()
    {
        return _objectCollections[_currentCollection].Objects.ToArray();
    }

    private void ChangeCollection(int index)
    {
        _currentObject = 0;
        _currentCollection = (_currentCollection + _objectCollections.Count + index) % _objectCollections.Count;
        OnCollectionChanged?.Invoke();
    }

    public void StartPlacing(bool started)
    {
        OnStateChanged?.Invoke(started);
    }
}