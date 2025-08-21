using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Diagnostics;
using workshop.wwwapi.DTOs;
using workshop.wwwapi.Models;

namespace workshop.tests;

public class Tests
{

    [Test] // Test that endpoint for getting patients works correctly and contains seeded patients
    public async Task GetPatients()
    {
        // Arrange: prepare request data
        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        var client = factory.CreateClient();

        // Act: make an API call using the shared _client instance
        var response = await client.GetAsync("/patients"); // send a GET request to the /patients endpoint to get all patients

        // Assert: check the response
        Assert.That(response.StatusCode == System.Net.HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync(); // read the response content as string
        var patients = System.Text.Json.JsonSerializer.Deserialize<List<PatientWithAppointmentsDto>>(content); // deserialize the JSON response to a list of Patients
        Assert.That(patients, Is.Not.Null);
        Assert.That(patients.Count, Is.GreaterThanOrEqualTo(2)); // check that there are at least 2 patients

        patients.ForEach(p => TestContext.WriteLine(p.FullName));

        Assert.That(patients.Any(p => p.FullName == "Lionel Messi")); // check that Lionel Messi is in the list


    }

    [Test] // Test that endpoint for getting a patient by ID returns the correct patient
    public async Task GetPatientById()
    {
        // Arrange: prepare request data
        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        var client = factory.CreateClient();

        // Act: make an API call using the shared _client instance
        var response = await client.GetAsync("/patients/1");

        // Assert: check the response
        Assert.That(response.StatusCode == System.Net.HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync(); // read the response content as string
        var patient = System.Text.Json.JsonSerializer.Deserialize<PatientWithAppointmentsDto>(content); // deserialize the JSON response to a PatientWithAppointmentsDto
        Assert.That(patient, Is.Not.Null);
        Assert.That(patient.FullName, Is.EqualTo("Lionel Messi"));
    }

    [Test] // Test that endpoint for adding a new patient works correctly
    public async Task AddPatient()
    {
        // Arrange: prepare request data
        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        var client = factory.CreateClient();
        var newPatient = new PersonPostDto { FullName = "Dimitar Berbatov" };
        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(newPatient), System.Text.Encoding.UTF8, "application/json");
        
        // Act: make an API call using the shared _client instance
        var response = await client.PostAsync("/patients", content);
        
        // Assert: check the response
        Assert.That(response.StatusCode == System.Net.HttpStatusCode.Created);

        var getResponse = await client.GetAsync("/patients"); // get all patients to check if the new patient was added
        var getContent = await getResponse.Content.ReadAsStringAsync();
        var patients = System.Text.Json.JsonSerializer.Deserialize<List<PatientWithAppointmentsDto>>(getContent);
        Assert.That(patients, Is.Not.Null);
        Assert.That(patients.Any(p => p.FullName == "Dimitar Berbatov")); // check that the new patient is in the list
    }


}