using Newtonsoft.Json;

namespace com.epam.rp_nunit.Core
{
    /*public static class TestDataLoader
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
    }*/
    
    public static class TestDataLoader
    {
        private static readonly string BasePath = Path.Combine(AppContext.BaseDirectory, "Core", "TestData");

        public static List<T> LoadTestData<T>(string fileName, string testName)
        {
            var filePath = Path.Combine(BasePath, fileName);
            
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Test data file not found: {filePath}");
            
            var json = File.ReadAllText(filePath);
            var allData = JsonConvert.DeserializeObject<Dictionary<string, List<T>>>(json);

            if (allData != null && allData.ContainsKey(testName))
            {
                var originalList = allData[testName];
                var copies = originalList
                    .Select(item => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(item)))
                    .ToList();
                return copies;
            }

            return new List<T>();
        }
    }
}
