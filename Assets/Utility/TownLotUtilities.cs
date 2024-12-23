using Controllers;
using Placement;
using Town.TownObjects;

namespace Utility
{
    /// <summary>
    /// A utility to handle displaying several details about town lots
    /// </summary>
    public static class TownLotUtilities
    {
        private static string ColorTextCount(int count, int max)
        {
            return $"<color=#{ColorPaletteUtilities.YellowHex}>{count}</color> / {max}";
        }

        public static string ColoredVisitorText(this TownLot lot)
        {
            return $"Capacity: {(lot.CanHaveVisitors() ? ColorTextCount(lot.GetVisitorCount(), lot.GetMaxVisitorCapacity()) : "")}\n{lot.GetVisitorCriteria()}";
        }

        public static string ColoredEmployeeText(this Workplace lot)
        {
            return $"Capacity: {ColorTextCount(lot.EmployeeCount, lot.MaxEmployeeCapacity)}\n{lot.GetWorkCriteria()}";
        }

        public static string ColoredInhabitantsText(this House house)
        {
            return $"Capacity: {ColorTextCount(house.GetInhabitantsCount(), house.MaxHouseholdCapacity)}";
        }
    }
}