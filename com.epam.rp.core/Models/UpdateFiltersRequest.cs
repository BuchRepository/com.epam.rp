namespace com.epam.rp.core.Models;

public class UpdateFiltersRequest
{
    public List<UpdateFilterElement> Elements { get; set; } = new List<UpdateFilterElement>();
}