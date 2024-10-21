using System;
using System.Collections.Generic;
using Common.Utility;
using UnityEngine;

public class TownObjectManager : MonoBehaviour
{
    [SerializeField] private List<ObjectCollection> _objectCollections;
    [SerializeField] private ObjectCollection _currentCollection;
    private int _currentObject = 0;
    public TownObj CurrentObject => _currentCollection.Objects.Count == 0 || _currentObject < 0 ? null : _currentCollection.Objects[_currentObject];
    public event Action OnCollectionChanged;
    public event Action OnSelectionChanged;
    public event Action<bool> OnSelectionStateChanged;

    public void SetObjectSelection(int index)
    {
        _currentObject = index == -1 ? -1 : (int)Mathf.Repeat(index, _currentCollection.Objects.Count);
        OnSelectionChanged?.Invoke();
    }

    public TownObj[] GetObjectsInCollection()
    {
        return _currentCollection.Objects.ToArray();
    }

    public void ChangeCollection(int index)
    {
        if(!index.IsBetweenRange(0, _objectCollections.Count-1))
        {
            Debug.LogWarning($"{nameof(TownObjectManager)}: Index is out of bounds");
            return;
        }

        _currentObject = 0;
        _currentCollection = _objectCollections[index];
        OnCollectionChanged?.Invoke();
    }

    public void StartSelection(bool started)
    {
        OnSelectionStateChanged?.Invoke(started);
    }
}