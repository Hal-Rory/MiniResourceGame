using Placement;
using Town.TownObjectData;

namespace Town.TownObjects
{
    public class CasualLot : TownLot
    {
        private CasualLotObj _casualLotObj => _townLotData as CasualLotObj;
    }
}