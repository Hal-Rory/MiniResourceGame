using System.Collections.Generic;
using System.Linq;
using Town.TownPopulation;
using UnityEngine;

public class WorkplaceManager : MonoBehaviour
{
    public List<Workplace> Workplaces { get; private set; } = new List<Workplace>();

    private void OnEnable()
    {
        GameController.Instance.RegisterPlacementListener(ObjectPlacerOnOnLotAdded, ObjectPlacerOnOnLotRemoved);
    }

    private void OnDisable()
    {
        GameController.Instance.UnregisterPlacementListener(ObjectPlacerOnOnLotAdded, ObjectPlacerOnOnLotRemoved);
    }

    private void ObjectPlacerOnOnLotAdded(TownLot obj)
    {
        if (obj is Workplace workplace)
        {
            CreateWorkplace(workplace);
        }
    }

    private void ObjectPlacerOnOnLotRemoved(TownLot obj)
    {
        if (obj is Workplace workplace)
        {
            ShutdownWorkplace(workplace);
        }
    }

    public void CreateWorkplace(Workplace workplace)
    {
        Workplaces.Add(workplace);
    }

    public bool Hire(Person person)
    {
        if (Workplaces.Count == 0) return false;
        List<Workplace> workplaces = Workplaces.FindAll(workplace => workplace.AgeGroups.Contains(person.AgeGroup));
        if (workplaces.Count == 0) return false;
        int job = Random.Range(0, workplaces.Count);
        Workplaces[job].Employ(person);
        return true;
    }

    public void ShutdownWorkplace(Workplace workplace)
    {
        Workplaces.Remove(workplace);
        workplace.UnemployAll();
    }
}