using Microsoft.AspNetCore.Mvc;
using workshop.wwwapi.DTOs;
using workshop.wwwapi.Repository;

namespace workshop.wwwapi.Endpoints
{
    public static class PatientEndpoints
    {
        //TODO:  add additional endpoints in here according to the requirements in the README.md 
        public static void ConfigurePatientEndpoint(this WebApplication app)
        {
            var patients = app.MapGroup("patients");

            patients.MapGet("/", GetPatients);
            patients.MapGet("/{id}", GetPatientById);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> GetPatients(IRepository repository)
        { 
            var patients = await repository.GetPatients();
            if (patients == null || !patients.Any()) { return TypedResults.NotFound("No patients found."); }

            var patientDto = patients.Select(p => new PatientDto { FullName = p.FullName, Id = p.Id }).ToList();
            return TypedResults.Ok(patientDto);
        }

        public static async Task<IResult> GetPatientById(int id, IRepository repository)
        {
            var patients = await repository.GetPatients();
            var patient = patients.FirstOrDefault(p => p.Id == id);
            if (patient == null) { return TypedResults.NotFound($"Patient with ID {id} not found."); }
            var patientDto = new PatientDto { FullName = patient.FullName, Id = patient.Id };
            return TypedResults.Ok(patientDto);
        }






    }
}
