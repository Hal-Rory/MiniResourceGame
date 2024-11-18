using System.Collections.Generic;
using Interfaces;
using Town.TownPopulation;
using UnityEngine;

public class WorkplaceManager : ITimeListener, IControllable
{
    public List<Workplace> Workplaces { get; private set; } = new List<Workplace>();

    private PopulationFactory _population => GameController.Instance.Population;

    public void SetUp()
    {
        GameController.Instance.RegisterPlacementListener(ObjectPlacerOnOnLotAdded, ObjectPlacerOnOnLotRemoved);
        GameController.Instance.GameTime.RegisterListener(this, true);
        _population.OnPopulationChanged += PopulationCheck;
    }

    public void SetDown()
    {
        GameController.Instance.UnregisterPlacementListener(ObjectPlacerOnOnLotAdded, ObjectPlacerOnOnLotRemoved);
        GameController.Instance.GameTime.UnregisterListener(this, true);
        _population.OnPopulationChanged -= PopulationCheck;
    }

    private void PopulationCheck(IPopulation population)
    {
        if(population is Person { JobIndex: >= 0 } person)
            Workplaces.Find(workplace => workplace.PlacementID == person.JobIndex).Unemploy(person);
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
        List<Person> unemployed = _population.Population.FindAll(person => person.JobIndex == -1 && person.CanWork);
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