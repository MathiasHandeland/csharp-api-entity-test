using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace workshop.tests;

public class Tests
{

    [Test] // Test that endpoint for getting patients returns a list of patients
    public async Task GetPatients()
    {
        // Arrange: prepare request data
        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        var client = factory.CreateClient();

        // Act: make an API call using the shared _client instance
        var response = await client.GetAsync("/patients");

        // Assert: check the response
        Assert.That(response.StatusCode == System.Net.HttpStatusCode.OK);

    
    }
}