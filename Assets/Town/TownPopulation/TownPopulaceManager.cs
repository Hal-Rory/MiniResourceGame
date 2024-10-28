using System.Collections.Generic;
using Town.TownPopulation;

/// <summary>
/// Acts as the middle man between the game controller(stockpiles) and the people.
/// Handles all things related to the population such as dishing out resources.
/// </summary>
public class TownPopulaceManager : IControllable, ITimeListener
{
    private Dictionary<PersonStats, float> _stockpiles;
    public void SetUp()
    {
        _stockpiles = PopulationUtility.StatSetup();
        _stockpiles[PersonStats.Happiness] = 100;
        _stockpiles[PersonStats.Health] = 100;
        _stockpiles[PersonStats.Hunger] = 100;
        GameController.Instance.GameTime.RegisterListener(this, true);
    }

    public void SetDown()
    {
        GameController.Instance.GameTime.UnregisterListener(this, true);
    }

    public void ClockUpdate(int tick)
    {
        AdjustStockpiles();
    }

    private void AdjustStockpiles()
    {

    }
}