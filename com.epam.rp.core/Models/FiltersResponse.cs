namespace com.epam.rp.core.Models;

public class FiltersResponse
{
    public List<FilterItem> Content { get; set; } = new();
    public PageInfo Page { get; set; } = null!;
}