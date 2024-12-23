using System.Collections.Generic;
using UnityEngine;

namespace Town.TownObjectData
{
    /// <summary>
    /// A collection of town lots and the name of that collection
    /// </summary>
    [CreateAssetMenu(fileName = "Town Object Collection", menuName = "Town/Create Town Object Collection")]
    public class ObjectCollection : ScriptableObject
    {
        public string Name;
        public List<TownLotObj> Objects;
    }
}