using System.Collections.Generic;
using System.Linq;
using Common.Utility;
using Controllers;
using Interfaces;
using Placement;
using Town.TownObjects;
using Town.TownPopulation;

namespace Town.TownObjectManagement
{
    public class WorkplaceManager : IControllable
    {
        public Dictionary<int, Workplace> Workplaces { get; } = new Dictionary<int, Workplace>();
        private PopulationFactory _population => GameController.Instance.Population;

        public void SetUp()
        {
            GameController.Instance.RegisterPlacementListener(ObjectPlacerOnOnLotAdded, ObjectPlacerOnOnLotRemoved);
            GameController.Instance.GameTime.RegisterListener(earlyClockUpdate: ClockUpdate);
        }

        public void SetDown()
        {
            GameController.Instance.UnregisterPlacementListener(ObjectPlacerOnOnLotAdded, ObjectPlacerOnOnLotRemoved);
            GameController.Instance.GameTime.UnregisterListener(earlyClockUpdate: ClockUpdate);
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
            Workplaces.Add(workplace.PlacementID, workplace);
        }

        private void Hire(Person person)
        {
            if (Workplaces.Count == 0) return;
            Workplace[] workplaces = Workplaces.Where(pair => pair.Value.CanHire(person)).Select(pair => pair.Value).ToArray();
            if (workplaces.Length == 0) return;
            workplaces.GetRandomIndex().Employ(person);
        }

        private void ShutdownWorkplace(Workplace workplace)
        {
            workplace.UnemployAll();
            Workplaces.Remove(workplace.PlacementID);
        }

        private void HireUnemployed()
        {
            List<Person> unemployed = _population.GetActivePopulation().FindAll(person => person.JobIndex == -1 && person.CanWork);
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
}