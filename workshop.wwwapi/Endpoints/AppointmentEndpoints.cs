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
            appointments.MapGet("/patients/{patientId}", GetAppointmentsByPatientId);
            appointments.MapGet("/doctors/{doctorId}", GetAppointmentByDoctorId);
            appointments.MapPost("/", AddAppointment);

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
                DoctorName = doctors.FirstOrDefault(d => d.Id == a.DoctorId)?.FullName,
                Booking = a.Booking
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
                PatientName = patient.FullName,
                DoctorName = doctor.FullName,
                Booking = appointment.Booking
            };

            return Results.Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> GetAppointmentsByPatientId(int patientId, IRepository<Appointment> appointmentRepository, IRepository<Patient> patientRepository, IRepository<Doctor> doctorRepository)
        {
            var patient = await patientRepository.GetById(patientId);
            if (patient == null) { return Results.NotFound($"Patient with ID {patientId} not found."); }
            var appointments = await appointmentRepository.GetAll();
            var doctors = await doctorRepository.GetAll();
            var patientAppointments = appointments.Where(a => a.PatientId == patientId);
            if (!patientAppointments.Any()) { return Results.NotFound($"No appointments found for patient with ID {patientId}."); }
            var result = patientAppointments.Select(a => new AppointmentWithDetailsDto
            {
                PatientName = patient.FullName,
                DoctorName = doctors.FirstOrDefault(d => d.Id == a.DoctorId)?.FullName,
                Booking = a.Booking
            });

            return Results.Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> GetAppointmentByDoctorId(int doctorId, IRepository<Appointment> appointmentRepository, IRepository<Patient> patientRepository, IRepository<Doctor> doctorRepository)
        {
            var doctor = await doctorRepository.GetById(doctorId);
            if (doctor == null) { return Results.NotFound($"Doctor with ID {doctorId} not found."); }
            var appointments = await appointmentRepository.GetAll();
            var patients = await patientRepository.GetAll();
            var doctorAppointments = appointments.Where(a => a.DoctorId == doctorId);
            if (!doctorAppointments.Any()) { return Results.NotFound($"No appointments found for doctor with ID {doctorId}."); }

            var result = doctorAppointments.Select(a => new AppointmentWithDetailsDto
            {
                PatientName = patients.FirstOrDefault(p => p.Id == a.PatientId)?.FullName,
                DoctorName = doctor.FullName,
                Booking = a.Booking
            });

            return Results.Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public static async Task<IResult> AddAppointment(AppointmentPostDto appointmentPostDto, IRepository<Appointment> appointmentRepository, IRepository<Patient> patientRepository, IRepository<Doctor> doctorRepository, HttpRequest request)
        {
            if (appointmentPostDto == null || appointmentPostDto.PatientId <= 0 || appointmentPostDto.DoctorId <= 0) { return Results.BadRequest("Invalid appointment data."); }
            var newAppointment = new Appointment
            {
                PatientId = appointmentPostDto.PatientId,
                DoctorId = appointmentPostDto.DoctorId,
                Booking = appointmentPostDto.Booking
            };
            var addedAppointment = await appointmentRepository.Add(newAppointment);

            var appointmentDto = new AppointmentWithDetailsDto
            {
                PatientName = (await patientRepository.GetById(addedAppointment.PatientId))?.FullName,
                DoctorName = (await doctorRepository.GetById(addedAppointment.DoctorId))?.FullName,
                Booking = addedAppointment.Booking
            };

            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var location = $"{baseUrl}/patients/{addedAppointment.Id}";
            return TypedResults.Created(location, appointmentDto);
        }

            
    }
}
