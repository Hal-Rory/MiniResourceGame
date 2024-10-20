using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Town Object Collection", menuName = "Town/Create Town Object Collection")]
public class ObjectCollection : ScriptableObject
{
    public string Name;
    public List<TownObj> Objects;
}