using Controllers;
using Placement;
using Town.TownObjects;

namespace Utility
{
    public static class TownLotUtilities
    {
        private static string ColorTextCount(int count, int max)
        {
            return $"<color=#{ColorPaletteUtilities.YellowHex}>{count}</color> / {max}";
        }

        public static string ColoredPersonsText(this TownLot lot)
        {
            return ColorTextCount(lot.GetVisitorCount(), lot.GetMaxVisitorCapacity());
        }

        public static string ColoredVisitorText(this TownLot lot)
        {
            return $"[Age Groups: {lot.GetVisitorCriteria()}]\n{(lot.CanHaveVisitors() ? ColorTextCount(lot.GetVisitorCount(), lot.GetMaxVisitorCapacity()) : "")}";
        }

        public static string ColoredEmployeeText(this Workplace lot)
        {
            return $"[Age Groups: {lot.GetWorkCriteria()}]\n{ColorTextCount(lot.EmployeeCount, lot.MaxEmployeeCapacity)}";
        }

        public static string ColoredInhabitantsText(this House house)
        {
            return $"{ColorTextCount(house.GetInhabitantsCount(), house.MaxHouseholdCapacity)}";
        }
    }
}