using System.Collections.Generic;
using Placement;
using Town.TownPopulation;

namespace Town.TownObjects
{
    public class CasualLot : TownLot
    {
        private List<Person> _patrons = new List<Person>();
        public override List<Person> GetPersons()
        {
            return _patrons;
        }

        public override int GetPersonsCount()
        {
            return _patrons.Count;
        }
    }
}