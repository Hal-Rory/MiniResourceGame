using System;
using System.Collections.Generic;
using System.Linq;
using Common.Utility;
using Controllers;
using Interfaces;
using Placement;
using Town.TownManagement;
using Town.TownObjects;
using Town.TownPopulation;

namespace Town.TownObjectManagement
{
    public class WorkplaceManager : IControllable
    {
        public Dictionary<int, Workplace> Workplaces { get; } = new Dictionary<int, Workplace>();
        private PopulationFactory _population => GameController.Instance.Population;
        public event Action<Workplace> OnWorkplaceUpdated;
        public int TotalWorkforce;

        public void SetUp()
        {
            GameController.Instance.RegisterPlacementListener(OnLotAdded, OnLotRemoved);
            GameController.Instance.GameTime.RegisterListener(earlyClockUpdate: ClockUpdate);
            _population.OnPopulationRemoved += OnPopulationRemoved;
        }

        public void SetDown()
        {
            GameController.Instance.UnregisterPlacementListener(OnLotAdded, OnLotRemoved);
            GameController.Instance.GameTime.UnregisterListener(earlyClockUpdate: ClockUpdate);
            if(_population != null) _population.OnPopulationRemoved -= OnPopulationRemoved;
        }

        /// <summary>
        /// When the population is modified by a remove, remove the people from any workplaces
        /// </summary>
        /// <param name="population"></param>
        private void OnPopulationRemoved(IPopulation population)
        {
            switch (population)
            {
                case Household household:
                    foreach (Person inhabitant in household.GetInhabitants())
                    {
                        if (inhabitant.JobIndex == -1) continue;
                        Workplaces[inhabitant.JobIndex].Unemploy(inhabitant);
                        TotalWorkforce--;
                    }
                    break;
                case Person person:
                    if (person.JobIndex == -1) return;
                    Workplaces[person.JobIndex].Unemploy(person);
                    TotalWorkforce--;
                    break;
            }
        }

        private void OnLotAdded(TownLot obj)
        {
            if (obj is Workplace workplace)
            {
                CreateWorkplace(workplace);
            }
        }

        private void OnLotRemoved(TownLot obj)
        {
            if (obj is Workplace workplace)
            {
                ShutdownWorkplace(workplace);
            }
        }

        private void CreateWorkplace(Workplace workplace)
        {
            Workplaces.Add(workplace.PlacementID, workplace);
        }

        private void Hire(Person person)
        {
            if (Workplaces.Count == 0) return;
            Workplace[] workplaces = Workplaces.Where(pair => pair.Value.CanHire(person)).Select(pair => pair.Value).ToArray();
            if (workplaces.Length == 0) return;
            Workplace workplace = workplaces.GetRandomIndex();
            workplace.Employ(person);
            TotalWorkforce++;
            OnWorkplaceUpdated?.Invoke(workplace);
        }

        private void ShutdownWorkplace(Workplace workplace)
        {
            workplace.UnemployAll();
            foreach (Person employee in workplace.GetEmployees())
            {
                _population.RelocatePerson(employee, GameController.Instance.GameTime.TimeOfDay);
            }
            workplace.ShutdownWorkplace();
            Workplaces.Remove(workplace.PlacementID);
        }

        /// <summary>
        /// Find all unemployed persons and place them in a viable workplace
        /// </summary>
        private void HireUnemployed()
        {
            List<Person> unemployed = _population.GetActivePopulation().FindAll(person => person.JobIndex == -1 && person.CanWork);
            if (unemployed.Count == 0) return;
            foreach (Person person in unemployed)
            {
                Hire(person);
            }
        }

        /// <summary>
        /// Early clock update, attempt to hire any eligible unemployed persons
        /// </summary>
        /// <param name="tick"></param>
        private void ClockUpdate(int tick)
        {
            HireUnemployed();
        }
    }
}