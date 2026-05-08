// Simple client-side DTOs for Prescription display
// These are independent copies not tied to the API project

namespace ClinicalPatientManagement.Client.Models;

public class PrescriptionViewModel
{
    public int Id { get; set; }
    public int ConsultationId { get; set; }
    public DateTime PrescriptionDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<MedicationViewModel> Medications { get; set; } = new();
    public ConsultationViewModel? Consultation { get; set; }
}

public class MedicationViewModel
{
    public int Id { get; set; }
    public int PrescriptionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Instructions { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ConsultationViewModel
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public decimal Temperature { get; set; }
    public string BloodPressure { get; set; } = string.Empty;
    public int Pulse { get; set; }
    public string Complaints { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public AppointmentViewModel? Appointment { get; set; }
}

public class AppointmentViewModel
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public PatientViewModel? Patient { get; set; }
}

public class PatientViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
