using System.Collections.Generic;
using Controllers;
using Interfaces;
using Placement;
using Town.TownPopulation;
using UnityEngine;

public class WorkplaceManager : IControllable
{
    public List<Workplace> Workplaces { get; } = new List<Workplace>();
    private PopulationFactory _population => GameController.Instance.Population;

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
            Hire(person);
        }
    }

    public void ClockUpdate(int tick)
    {
        HireUnemployed();
    }
}