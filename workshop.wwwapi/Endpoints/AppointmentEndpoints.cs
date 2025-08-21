using Microsoft.AspNetCore.Mvc;
using workshop.wwwapi.DTOs;
using workshop.wwwapi.Models;
using workshop.wwwapi.Repository;

namespace workshop.wwwapi.Endpoints
{
    public static class AppointmentEndpoints
    {
        public static void ConfigureAppointmentEndpoint(this WebApplication app)
        {
            var appointments = app.MapGroup("appointments");

            appointments.MapGet("/", GetAppointments);
            appointments.MapGet("/{id}", GetAppointmentById);

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        public static async Task<IResult> GetAppointments(IRepository<Appointment> appointmentRepository, IRepository<Patient> patientRepository, IRepository<Doctor> doctorRepository)
        {
            var appointments = await appointmentRepository.GetAll();
            var patients = await patientRepository.GetAll();
            var doctors = await doctorRepository.GetAll();

            var result = appointments.Select(a => new AppointmentWithDetailsDto
            {
                PatientName = patients.FirstOrDefault(p => p.Id == a.PatientId)?.FullName,
                DoctorName = doctors.FirstOrDefault(d => d.Id == a.DoctorId)?.FullName
            });

            return Results.Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> GetAppointmentById(int id, IRepository<Appointment> appointmentRepository, IRepository<Patient> patientRepository, IRepository<Doctor> doctorRepository)
        {
            var appointment = await appointmentRepository.GetById(id);
            if (appointment == null) { return Results.NotFound($"Appointment with ID {id} not found."); }
            var patient = await patientRepository.GetById(appointment.PatientId);
            var doctor = await doctorRepository.GetById(appointment.DoctorId);

            var result = new AppointmentWithDetailsDto
            {
                PatientName = patient?.FullName,
                DoctorName = doctor?.FullName
            };
            return Results.Ok(result);
        }
    }
}
