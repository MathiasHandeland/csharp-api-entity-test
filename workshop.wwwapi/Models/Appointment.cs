using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workshop.wwwapi.Models
{
    //TODO: decorate class/columns accordingly
    [Table("appointments")]
    public class Appointment
    {

        [Column("booking_date")]
        public DateTime Booking { get; set; }

        [Column("doctor_id")]
        public int DoctorId { get; set; }

        [Column("patient_id")]
        public int PatientId { get; set; }


        [ForeignKey("DoctorId")] // Foreign key to Doctor table
        public Doctor Doctor { get; set; } // Navigation property

        [ForeignKey("PatientId")] // Foreign key to Patient table
        public Patient Patient { get; set; } // Navigation property

    }
}
