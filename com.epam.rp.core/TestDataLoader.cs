using Newtonsoft.Json;

namespace com.epam.rp.core
{
    
    public static class TestDataLoader
    {
        private static readonly string BasePath = Path.Combine(AppContext.BaseDirectory, "Core", "TestData");

        public static List<T> LoadTestData<T>(string fileName, string testName)
        {
            var filePath = Path.Combine(BasePath, fileName);
            
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Test data file not found: {filePath}");
            
            var json = File.ReadAllText(filePath);
            
            Dictionary<string, List<T>>? allData;
            try
            {
                allData = JsonConvert.DeserializeObject<Dictionary<string, List<T>>>(json);
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Failed to deserialize test data file '{filePath}'. Check JSON format.", ex);
            }

            if (allData == null)
                throw new JsonException($"Test data file '{filePath}' is empty or invalid.");

            return allData.TryGetValue(testName, out var list) ? list.ToList() : new List<T>();
        }
    }
}
