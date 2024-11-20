using System.Collections.Generic;
using Town.TownObjectData;
using UnityEngine;

[CreateAssetMenu(fileName = "Town Object Collection", menuName = "Town/Create Town Object Collection")]
public class ObjectCollection : ScriptableObject
{
    public string Name;
    public List<TownLotObj> Objects;
}