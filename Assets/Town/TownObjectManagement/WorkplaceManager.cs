using System;
using System.Collections.Generic;
using Controllers;
using Interfaces;
using Placement;
using Town.TownObjects;
using Town.TownPopulation;
using Random = UnityEngine.Random;

public class WorkplaceManager : IControllable
{
    public List<Workplace> Workplaces { get; } = new List<Workplace>();
    private PopulationFactory _population => GameController.Instance.Population;
    public event Action OnWorkforceChanged;

    public void SetUp()
    {
        GameController.Instance.RegisterPlacementListener(ObjectPlacerOnOnLotAdded, ObjectPlacerOnOnLotRemoved);
        GameController.Instance.GameTime.RegisterListener(earlyClockUpdate: ClockUpdate);
        _population.OnPopulationChanged += PopulationCheck;
    }

    public void SetDown()
    {
        GameController.Instance.UnregisterPlacementListener(ObjectPlacerOnOnLotAdded, ObjectPlacerOnOnLotRemoved);
        GameController.Instance.GameTime.UnregisterListener(ClockUpdate);
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

    private void CreateWorkplace(Workplace workplace)
    {
        Workplaces.Add(workplace);
    }

    private void Hire(Person person)
    {
        if (Workplaces.Count == 0) return;
        List<Workplace> workplaces = Workplaces.FindAll(workplace => workplace.CanHire(person));
        if (workplaces.Count == 0) return;
        int job = Random.Range(0, workplaces.Count);
        workplaces[job].Employ(person);
        OnWorkforceChanged?.Invoke();
    }

    private void ShutdownWorkplace(Workplace workplace)
    {
        Workplaces.Remove(workplace);
        workplace.UnemployAll();
    }

    private void HireUnemployed()
    {
        List<Person> unemployed = _population.Population.FindAll(person => person.JobIndex == -1 && person.CanWork);
        if (unemployed.Count == 0) return;
        foreach (Person person in unemployed)
        {
            Hire(person);
        }
    }

    private void ClockUpdate(int tick)
    {
        HireUnemployed();
    }
}