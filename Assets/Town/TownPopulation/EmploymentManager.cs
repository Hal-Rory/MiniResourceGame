using System.Collections.Generic;
using Town.TownPopulation;
using UnityEngine;

public class EmploymentManager : MonoBehaviour, ITimeListener
{
    private PopulationManager _populationManager => GameController.Instance.PopulationManager;
    private WorkplaceManager _workplaceManager => GameController.Instance.WorkplaceManager;

    private void Start()
    {
        GameController.Instance.TimeManager.RegisterListener(this);
    }

    public void HireUnemployed()
    {
        List<Person> unemployed = _populationManager.Population.FindAll(person => person.JobIndex == -1 && person.CanWork);
        if (unemployed.Count == 0) return;
        foreach (Person person in unemployed)
        {
            if(_workplaceManager.Hire(person))
                print($"{person.Name} from household {person.HouseholdIndex} was just hired");
        }
    }

    public void ClockUpdate(int tick)
    {
        HireUnemployed();
    }
}