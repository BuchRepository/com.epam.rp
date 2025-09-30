using System.Net;
using AventStack.ExtentReports;
using com.epam.rp.core;
using com.epam.rp.core.Models;
using com.epam.rp.core.Utility;
using Newtonsoft.Json;

namespace com.epam.rp.api.Tests;

[TestFixture]
public class FiltersApiTests
{
    private FiltersApiClient _apiClient = new FiltersApiClient();
    private ExtentReports? _extent;
    private ExtentTest? _test;
    
    const int InvaliId = 123456789;
    
    public void OneTimeSetup()
    {
        LoggerService.InitLogger();
        _apiClient = new FiltersApiClient();

        _extent = ReportManager.GetExtent(isUI: false);
    }

    [SetUp]
    public void Setup()
    {
        _test = _extent!.CreateTest(TestContext.CurrentContext.Test.Name);
        LoggerService.Info($"Starting test: {TestContext.CurrentContext.Test.Name}");
    }

    [Test]
    public async Task GetAllFilters_Positive()
    {
        var response = await _apiClient.GetFiltersAsync();
        LoggerService.Info($"Request completed. Status code: {response.StatusCode}.");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var filtersResponse = JsonConvert.DeserializeObject<FiltersResponse>(response.Content!)!;
        Assert.That(filtersResponse, Is.Not.Null, "Response deserialization failed.");
        
        Assert.That(filtersResponse.Content.Count, Is.GreaterThan(0), "No filters found.");
        Assert.That(filtersResponse.Content[0].Name, Is.Not.Null.And.Not.Empty, "Filter Name is null or empty.");

        LoggerService.Info($"First filter name: {filtersResponse.Content[0].Name}");
    }
    
    [Test]
    public async Task GetFilterById_Positive()
    {
        var body = new CreateFilterRequest
        {
            Name = $"TestFilter_{Guid.NewGuid()}",
            Type = "launch",
            Conditions = new List<FilterCondition>
            {
                new FilterCondition
                {
                    FilteringField = "statistics$executions$total",
                    Condition = "gte",
                    Value = "1"
                }
            },
            Orders = new List<FilterOrder>
            {
                new FilterOrder
                {
                    SortingColumn = "startTime",
                    IsAsc = false
                }
            }
        };
        
        var createResponse = await _apiClient.CreateFilterAsync(body);
        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        
        var created = JsonConvert.DeserializeObject<CreateFilterResponse>(createResponse.Content!);
        Assert.That(created, Is.Not.Null);

        var filterId = created!.Id;

        var getResponse = await _apiClient.GetFilterByIdAsync(filterId);
        LoggerService.Info($"Request completed. Status code: {getResponse.StatusCode}.");

        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var response = await _apiClient.DeleteFilterAsync(filterId);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var deleteResponse = JsonConvert.DeserializeObject<DeleteFilterResponse>(response.Content!);
        Assert.That(deleteResponse, Is.Not.Null);
        Assert.That(deleteResponse!.Message, Is.EqualTo($"User filter with ID = '{filterId}' successfully deleted."));
    }
    
    [Test]
    public async Task GetFilterById_Negative_NotFound()
    {
        var response = await _apiClient.GetFilterByIdAsync(InvaliId);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreateFilter_Positive()
    {
        var body = new CreateFilterRequest
        {
            Name = $"TestFilter_{Guid.NewGuid()}",
            Type = "launch",
            Conditions = new List<FilterCondition>
            {
                new FilterCondition
                {
                    FilteringField = "statistics$executions$total",
                    Condition = "gte",
                    Value = "1"
                }
            },
            Orders = new List<FilterOrder>
            {
                new FilterOrder
                {
                    SortingColumn = "startTime",
                    IsAsc = false
                }
            }
        };
        
        var createResponse = await _apiClient.CreateFilterAsync(body);
        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        
        var created = JsonConvert.DeserializeObject<CreateFilterResponse>(createResponse.Content!);
        Assert.That(created, Is.Not.Null);

        var filterId = created!.Id;
        
        var response = await _apiClient.DeleteFilterAsync(filterId);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var deleteResponse = JsonConvert.DeserializeObject<DeleteFilterResponse>(response.Content!);
        Assert.That(deleteResponse, Is.Not.Null);
        Assert.That(deleteResponse!.Message, Is.EqualTo($"User filter with ID = '{filterId}' successfully deleted."));
    }

    [Test]
    public async Task CreateFilter_Negative_MissingName()
    {
        var body = new CreateFilterRequest
        {
            Name = null,
            Type = "launch",
            Conditions = new List<FilterCondition>
            {
                new FilterCondition
                {
                    FilteringField = "statistics$executions$total",
                    Condition = "gte",
                    Value = "1"
                }
            },
            Orders = new List<FilterOrder>
            {
                new FilterOrder
                {
                    SortingColumn = "startTime",
                    IsAsc = false
                }
            }
        };
        
        var response = await _apiClient.CreateFilterAsync(body);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task CreateFilter_Negative_InvalidType()
    {
        var body = new CreateFilterRequest
        {
            Name = $"TestFilter_{Guid.NewGuid()}",
            Type = "InvalidType",
            Conditions = new List<FilterCondition>
            {
                new FilterCondition
                {
                    FilteringField = "statistics$executions$total",
                    Condition = "gte",
                    Value = "1"
                }
            },
            Orders = new List<FilterOrder>
            {
                new FilterOrder
                {
                    SortingColumn = "startTime",
                    IsAsc = false
                }
            }
        };
        
        var response = await _apiClient.CreateFilterAsync(body);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task UpdateFilterById_Positive()
    {
        var createBody = new CreateFilterRequest
        {
            Name = $"TestFilter_{Guid.NewGuid()}",
            Type = "launch",
            Conditions = new List<FilterCondition>
            {
                new FilterCondition
                {
                    FilteringField = "statistics$executions$total",
                    Condition = "gte",
                    Value = "1"
                }
            },
            Orders = new List<FilterOrder>
            {
                new FilterOrder
                {
                    SortingColumn = "startTime",
                    IsAsc = false
                }
            }
        };
        
        var createResponse = await _apiClient.CreateFilterAsync(createBody);
        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        
        var created = JsonConvert.DeserializeObject<CreateFilterResponse>(createResponse.Content!);
        Assert.That(created, Is.Not.Null);

        var filterId = created!.Id;
        
        var updateBody = new UpdateFilterRequest
        {
            Name = "UpdatedName",
            Description = createBody.Description,
            Type = createBody.Type,
            Conditions = createBody.Conditions,
            Orders = createBody.Orders
        };
        
        var updateResponse = await _apiClient.UpdateFilterByIdAsync(filterId, updateBody);
        Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var getResponse = await _apiClient.GetFilterByIdAsync(filterId);
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var updatedFilter = JsonConvert.DeserializeObject<CreateFilterRequest>(getResponse.Content!);
        Assert.That(updatedFilter!.Name, Is.EqualTo("UpdatedName"));
        
        var deleteResponse = await _apiClient.DeleteFilterAsync(filterId);
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var deleteContent = JsonConvert.DeserializeObject<DeleteFilterResponse>(deleteResponse.Content!);
        Assert.That(deleteContent, Is.Not.Null);
        Assert.That(deleteContent!.Message, Is.EqualTo($"User filter with ID = '{filterId}' successfully deleted."));
    }

    [Test]
    public async Task UpdateFilterById_Negative_InvalidId()
    {
        LoggerService.Info($"Running UpdateFilterById_Negative_InvalidId.");
        
        var updateBody = new UpdateFilterRequest
       {
           Name = $"TestFilter_{Guid.NewGuid()}",
           Type = "launch",
           Conditions = new List<FilterCondition>
           {
               new FilterCondition
               {
                   FilteringField = "statistics$executions$total",
                   Condition = "gte",
                   Value = "1"
               }
           },
           Orders = new List<FilterOrder>
           {
               new FilterOrder
               {
                   SortingColumn = "startTime",
                   IsAsc = false
               }
           }
       };
        
        var response = await _apiClient.UpdateFilterByIdAsync(InvaliId, updateBody);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task UpdateFilters_Positive()
    {
        var createBody = new CreateFilterRequest
        {
            Name = $"TestFilter_{Guid.NewGuid()}",
            Type = "launch",
            Conditions = new List<FilterCondition>
            {
                new FilterCondition
                {
                    FilteringField = "statistics$executions$total",
                    Condition = "gte",
                    Value = "1"
                }
            },
            Orders = new List<FilterOrder>
            {
                new FilterOrder
                {
                    SortingColumn = "startTime",
                    IsAsc = false
                }
            }
        };
            
        var createResponse = await _apiClient.CreateFilterAsync(createBody);
        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        
        var created = JsonConvert.DeserializeObject<CreateFilterResponse>(createResponse.Content!);
        Assert.That(created, Is.Not.Null);

        var filterId = created!.Id;
        
        var updateBody = new UpdateFiltersRequest
        {
            Elements = new List<UpdateFilterElement>
            {
                new UpdateFilterElement()
                {
                    Id = filterId,
                    Name = "UpdatedName",
                    Description = createBody.Description,
                    Type = createBody.Type,
                    Conditions = createBody.Conditions,
                    Orders = createBody.Orders
                }
            }
        };
        
        var updateResponse = await _apiClient.UpdateFiltersAsync(updateBody);
        
        Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var getResponse = await _apiClient.GetFilterByIdAsync(filterId);
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var updatedFilter = JsonConvert.DeserializeObject<CreateFilterRequest>(getResponse.Content!);
        Assert.That(updatedFilter!.Name, Is.EqualTo("UpdatedName"));
        
        var deleteResponse = await _apiClient.DeleteFilterAsync(filterId);
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var deleteContent = JsonConvert.DeserializeObject<DeleteFilterResponse>(deleteResponse.Content!);
        Assert.That(deleteContent, Is.Not.Null);
        Assert.That(deleteContent!.Message, Is.EqualTo($"User filter with ID = '{filterId}' successfully deleted."));
    }
    
    [Test]
    public async Task UpdateFilters_Negative_InvalidId()
    {
        var updateBody = new UpdateFiltersRequest
        {
            Elements = new List<UpdateFilterElement>
            {
                new UpdateFilterElement()
                {
                    Id = InvaliId,
                    Name = $"TestFilter_{Guid.NewGuid()}",
                    Type = "launch",
                    Conditions = new List<FilterCondition>
                    {
                        new FilterCondition
                        {
                            FilteringField = "statistics$executions$total",
                            Condition = "gte",
                            Value = "1"
                        }
                    },
                    Orders = new List<FilterOrder>
                    {
                        new FilterOrder
                        {
                            SortingColumn = "startTime",
                            IsAsc = false
                        }
                    }
                }
            }
        };
                    
        var updateResponse = await _apiClient.UpdateFiltersAsync(updateBody);
        
        Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task DeleteFilter_Positive()
    {
        var createBody = new CreateFilterRequest
        {
            Name = $"TestFilter_{Guid.NewGuid()}",
            Type = "launch",
            Conditions = new List<FilterCondition>
            {
                new FilterCondition
                {
                    FilteringField = "statistics$executions$total",
                    Condition = "gte",
                    Value = "1"
                }
            },
            Orders = new List<FilterOrder>
            {
                new FilterOrder
                {
                    SortingColumn = "startTime",
                    IsAsc = false
                }
            }
        };
        
        var createResponse = await _apiClient.CreateFilterAsync(createBody);
        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        
        var created = JsonConvert.DeserializeObject<CreateFilterResponse>(createResponse.Content!);
        Assert.That(created, Is.Not.Null);

        var filterId = created!.Id;
        
        var deleteResponse = await _apiClient.DeleteFilterAsync(filterId);
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var deleteContent = JsonConvert.DeserializeObject<DeleteFilterResponse>(deleteResponse.Content!);
        Assert.That(deleteContent, Is.Not.Null);
        Assert.That(deleteContent!.Message, Is.EqualTo($"User filter with ID = '{filterId}' successfully deleted."));
    }

    [Test]
    public async Task DeleteFilter_Negative_NotFound()
    {
        var response = await _apiClient.DeleteFilterAsync(InvaliId);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [TearDown]
    public void TearDown()
    {
        var outcome = TestContext.CurrentContext.Result.Outcome.Status;

        if (outcome == NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            _test!.Fail("Test Failed")
                .Fail(TestContext.CurrentContext.Result.Message);
        }
        else if (outcome == NUnit.Framework.Interfaces.TestStatus.Passed)
        {
            _test!.Pass("Test Passed");
        }
        else
        {
            _test!.Skip("Test Skipped");
        }

        _extent!.Flush();
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        ReportManager.FlushReports();
    }
}