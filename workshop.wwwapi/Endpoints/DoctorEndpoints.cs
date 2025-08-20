using Microsoft.AspNetCore.Mvc;
using workshop.wwwapi.DTOs;
using workshop.wwwapi.Models;
using workshop.wwwapi.Repository;

namespace workshop.wwwapi.Endpoints
{
    public static class DoctorEndpoints
    {
        public static void ConfigureDoctorEndpoint(this WebApplication app)
        {
            var doctors = app.MapGroup("doctors");

            doctors.MapGet("/", GetDoctors);
            doctors.MapGet("/{id}", GetDoctorById);
            doctors.MapPost("/", AddDoctor);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> GetDoctors(IRepository<Doctor> doctorRepository)
        {
            var doctors = await doctorRepository.GetAll();
            if (doctors == null || !doctors.Any()) { return TypedResults.NotFound("No doctors found."); }

            var patientDto = doctors.Select(d => new PersonDto { FullName = d.FullName, Id = d.Id }).ToList();
            return TypedResults.Ok(patientDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> GetDoctorById(int id, IRepository<Doctor> doctorRepository)
        {
            var doctors = await doctorRepository.GetAll();
            var doctor = doctors.FirstOrDefault(d => d.Id == id);
            if (doctor == null) { return TypedResults.NotFound($"Doctor with ID {id} not found."); }
            var patientDto = new PersonDto { FullName = doctor.FullName, Id = doctor.Id };
            return TypedResults.Ok(patientDto);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public static async Task<IResult> AddDoctor([FromBody] PersonPostDto doctorPostDto, IRepository<Doctor> doctorRepository, HttpRequest request)
        {
            if (doctorPostDto == null || string.IsNullOrWhiteSpace(doctorPostDto.FullName)) { return TypedResults.BadRequest("Invalid doctor data."); }
            var newDoctor = new Doctor { FullName = doctorPostDto.FullName };
            var addedDoctor = await doctorRepository.Add(newDoctor);

            var doctorDto = new PersonDto { Id = addedDoctor.Id, FullName = addedDoctor.FullName };

            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var location = $"{baseUrl}/patients/{addedDoctor.Id}";
            return TypedResults.Created(location, doctorDto);

        }
    }
}
