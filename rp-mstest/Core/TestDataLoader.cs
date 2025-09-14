using Newtonsoft.Json;

namespace com.epam.rp_mstest.Core;

public static class TestDataLoader
{
    private static string basePath = Path.Combine(AppContext.BaseDirectory, "Core", "TestData");

    public static List<T> LoadTestData<T>(string fileName, string testName)
    {
        var filePath = Path.Combine(basePath, fileName);
        
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Test data file not found: {filePath}");
        
        var json = File.ReadAllText(filePath);
        var allData = JsonConvert.DeserializeObject<Dictionary<string, List<T>>>(json);
        return allData != null && allData.ContainsKey(testName) ? allData[testName] : new List<T>();
    }
}