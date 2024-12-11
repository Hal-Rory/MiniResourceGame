using System.Collections.Generic;
using Placement;
using Town.TownPopulation;

namespace Town.TownObjects
{
    public class CasualLot : TownLot
    {
        private List<Person> _visitors = new List<Person>();
        public override List<Person> GetPersons()
        {
            return _visitors;
        }

        public override int GetPersonsCount()
        {
            return _visitors.Count;
        }
    }
}