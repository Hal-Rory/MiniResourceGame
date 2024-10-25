using System.Collections.Generic;
using Town.TownPopulation;
using UnityEngine;

public class WorkplaceManager : ITimeListener
{
    public List<Workplace> Workplaces { get; private set; } = new List<Workplace>();

    private PopulationFactory _populationFactory => GameController.Instance.PopulationFactory;

    private void Start()
    {
        GameController.Instance.TimeManager.RegisterListener(this, true);
    }

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
        List<Workplace> workplaces = Workplaces.FindAll(workplace => workplace.CanHire(person));
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

    public void HireUnemployed()
    {
        List<Person> unemployed = _populationFactory.Population.FindAll(person => person.JobIndex == -1 && person.CanWork);
        if (unemployed.Count == 0) return;
        foreach (Person person in unemployed)
        {
            if(Hire(person))
                Debug.Log($"{person.Name} from household {person.HouseholdIndex} was just hired");
        }
    }

    public void ClockUpdate(int tick)
    {
        HireUnemployed();
    }
}