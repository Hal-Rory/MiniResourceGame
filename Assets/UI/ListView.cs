using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ListView : MonoBehaviour
    {
        [SerializeField] private ScrollRect _listView;

        public Dictionary<string, GameObject> SpawnedObjects { get; private set; } =
            new Dictionary<string, GameObject>();

        private void Awake()
        {
            SpawnedObjects = new Dictionary<string, GameObject>();
        }

        public GameObject SpawnItem(string id, GameObject prefab)
        {
            GameObject obj = Instantiate(prefab, _listView.content);
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
            GameObject[] objs = SpawnedObjects.Values.ToArray();
            for (int i = 0; i < count; i++)
            {
                Destroy(objs[i]);
            }

            SpawnedObjects.Clear();
        }

        public void UpdateLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_listView.content.transform as RectTransform);
        }
    }
}