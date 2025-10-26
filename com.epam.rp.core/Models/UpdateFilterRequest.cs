namespace com.epam.rp.core.Models;

public class UpdateFilterRequest
{
    public string? Description { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public List<FilterCondition> Conditions { get; set; } = new List<FilterCondition>();
    public List<FilterOrder> Orders { get; set; } = new List<FilterOrder>();
}