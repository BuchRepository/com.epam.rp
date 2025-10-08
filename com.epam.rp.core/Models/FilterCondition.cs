namespace com.epam.rp.core.Models;

public class FilterCondition
{
    public string FilteringField { get; set; } = null!;
    public string Condition { get; set; } = null!;
    public string Value { get; set; } = null!;
}