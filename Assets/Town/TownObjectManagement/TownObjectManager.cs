using System;
using System.Collections.Generic;
using Common.Utility;
using UnityEngine;

public class TownObjectManager : MonoBehaviour
{
    [SerializeField] private List<ObjectCollection> _objectCollections;
    private int _currentCollection;
    private int _currentObject;
    public TownObj CurrentObject => _objectCollections[_currentCollection].Objects.Count == 0 || _currentObject < 0 ? null : _objectCollections[_currentCollection].Objects[_currentObject];
    public event Action OnCollectionChanged;
    public event Action OnSelectionChanged;
    public event Action<bool> OnStateChanged;

    private void Start()
    {
        _currentCollection = 0;
    }

    public int GetCurrentCollection()
    {
        return _currentCollection;
    }

    public void SetObjectSelection(int index)
    {
        _currentObject = index == -1 ? -1 : (int)Mathf.Repeat(index, _objectCollections[_currentCollection].Objects.Count);
        OnSelectionChanged?.Invoke();
    }

    public TownObj[] GetObjectsInCollection()
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