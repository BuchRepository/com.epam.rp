namespace com.epam.rp.core.Models;

public class FilterItem
{
    public string Owner { get; set; } = null!;
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<FilterCondition> Conditions { get; set; } = new();
    public List<FilterOrder> Orders { get; set; } = new();
    public string Type { get; set; } = null!;
}
