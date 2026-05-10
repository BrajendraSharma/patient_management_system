namespace ClinicalPatientManagement.Client.Models;

/// <summary>
/// Medication in a prescription
/// Step 12: Patient History - Prescription display
/// </summary>
public class PrescriptionMedicationModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Instructions { get; set; } = string.Empty;
}

/// <summary>
/// Prescription model for client-side operations
/// Step 12: Patient History - Load prescriptions for consultations
/// </summary>
public class PrescriptionModel
{
    public int Id { get; set; }
    public int ConsultationId { get; set; }
    public DateTime PrescriptionDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<PrescriptionMedicationModel> Medications { get; set; } = new();
}
