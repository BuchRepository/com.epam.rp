namespace com.epam.rp.core.Models;

public class FilterOrder
{
    public string SortingColumn { get; set; } = null!;
    public bool IsAsc { get; set; }
}