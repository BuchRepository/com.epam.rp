namespace com.epam.rp_mstest.Businnes.Models;

public class FilterModelMsTest
{
    public string FilterName { get; set; }
    public string Owner { get; set; }
    
    public FilterModelMsTest(string filterName, string owner)
    {
        FilterName = filterName;
        Owner = owner;
    }
}