namespace Business.Models;

public class FilterModel
{
    public string FilterName { get; set; }
    public string Owner { get; set; }
    
    public FilterModel(string filterName = "", string owner = "")
    {
        FilterName = filterName;
        Owner = owner;
    }
}