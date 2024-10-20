public interface IIncomeContributor
{
    public int GetIncomeContribution();
    public bool CanContribute { get; }
}