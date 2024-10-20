using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] private List<ObjectCollection> _objectCollections;
    [SerializeField] private ObjectCollection _currentCollection;
    private int _currentObject = 0;
    public TownObj CurrentObject => _currentCollection.Objects.Count == 0 ? null : _currentCollection.Objects[_currentObject];

    public void UpdateObjectSelection(int indexDelta)
    {
        _currentObject = (int)Mathf.Repeat(_currentObject + indexDelta, _currentCollection.Objects.Count);
    }
}