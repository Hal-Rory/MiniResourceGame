using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Object pool for UI
    /// </summary>
    public class ListView : MonoBehaviour
    {
        [SerializeField] private RectTransform _listView;

        public Dictionary<string, GameObject> SpawnedObjects { get; private set; } =
            new Dictionary<string, GameObject>();

        private Queue<GameObject> _orphans = new Queue<GameObject>();

        private void Awake()
        {
            SpawnedObjects = new Dictionary<string, GameObject>();
        }

        public GameObject SpawnItem(string id, GameObject prefab)
        {
            GameObject obj = _orphans.Count > 0 ? _orphans.Dequeue() : Instantiate(prefab, _listView);
            obj.SetActive(true);
            SpawnedObjects.Add(id, obj);
            return obj;
        }

        public GameObject GetObj(string ID)
        {
            return SpawnedObjects[ID];
        }

        public void ClearCards()
        {
            if (SpawnedObjects == null) return;
            int count = SpawnedObjects.Count;
            if (count == 0) return;
            foreach (KeyValuePair<string,GameObject> spawnedObj in SpawnedObjects)
            {
                Destroy(spawnedObj.Value);
            }
            foreach (GameObject orphan in _orphans)
            {
                if(orphan) Destroy(orphan);
            }
            _orphans.Clear();
            SpawnedObjects.Clear();
        }

        public void StashCards()
        {
            if (SpawnedObjects == null) return;
            int count = SpawnedObjects.Count;
            if (count == 0) return;
            foreach (KeyValuePair<string,GameObject> spawnedObj in SpawnedObjects)
            {
                _orphans.Enqueue(spawnedObj.Value);
                spawnedObj.Value.SetActive(false);
            }
            SpawnedObjects.Clear();
        }

        public void UpdateLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_listView);
        }
    }
}