using AutoMapper;
using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Models;
using ClinicalPatientManagement.Api.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicalPatientManagement.Api.Services;

/// <summary>
/// Service implementation for Prescription business logic
/// Step 10: Prescription Generation
/// </summary>
public class PrescriptionService : IPrescriptionService
{
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IConsultationRepository _consultationRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PrescriptionService> _logger;

    public PrescriptionService(
        IPrescriptionRepository prescriptionRepository,
        IConsultationRepository consultationRepository,
        IMapper mapper,
        ILogger<PrescriptionService> logger)
    {
        _prescriptionRepository = prescriptionRepository;
        _consultationRepository = consultationRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all prescriptions
    /// </summary>
    public async Task<IEnumerable<PrescriptionDto>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all prescriptions");
        var prescriptions = await Task.FromResult(_prescriptionRepository.GetAll().ToList());
        return _mapper.Map<IEnumerable<PrescriptionDto>>(prescriptions);
    }

    /// <summary>
    /// Get prescription by ID
    /// </summary>
    public async Task<PrescriptionDto?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching prescription with ID: {PrescriptionId}", id);
        var prescription = await _prescriptionRepository.GetByIdAsync(id);
        
        if (prescription == null)
        {
            _logger.LogWarning("Prescription not found with ID: {PrescriptionId}", id);
            return null;
        }

        return _mapper.Map<PrescriptionDto>(prescription);
    }

    /// <summary>
    /// Get prescription by consultation ID
    /// </summary>
    public async Task<PrescriptionDto?> GetByConsultationIdAsync(int consultationId)
    {
        _logger.LogInformation("Fetching prescription for consultation ID: {ConsultationId}", consultationId);
        var prescription = await _prescriptionRepository.GetByConsultationIdAsync(consultationId);
        
        if (prescription == null)
        {
            _logger.LogWarning("Prescription not found for consultation ID: {ConsultationId}", consultationId);
            return null;
        }

        return _mapper.Map<PrescriptionDto>(prescription);
    }

    /// <summary>
    /// Get all prescriptions for a patient
    /// </summary>
    public async Task<IEnumerable<PrescriptionDto>> GetByPatientIdAsync(int patientId)
    {
        _logger.LogInformation("Fetching prescriptions for patient ID: {PatientId}", patientId);
        var prescriptions = await _prescriptionRepository.GetByPatientIdAsync(patientId);
        return _mapper.Map<IEnumerable<PrescriptionDto>>(prescriptions);
    }

    /// <summary>
    /// Create a new prescription with medications
    /// </summary>
    public async Task<PrescriptionDto> CreateAsync(CreatePrescriptionDto dto)
    {
        _logger.LogInformation("Creating new prescription for consultation ID: {ConsultationId}", dto.ConsultationId);

        // Validate prescription data
        if (!ValidatePrescriptionData(dto, out var errors))
        {
            var errorMessage = string.Join("; ", errors);
            _logger.LogError("Prescription validation failed: {Errors}", errorMessage);
            throw new ArgumentException($"Invalid prescription data: {errorMessage}");
        }

        // Verify consultation exists
        var consultation = await _consultationRepository.GetByIdAsync(dto.ConsultationId);
        if (consultation == null)
        {
            _logger.LogWarning("Consultation not found for ID: {ConsultationId}", dto.ConsultationId);
            throw new ArgumentException($"Consultation with ID {dto.ConsultationId} not found");
        }

        // Check if prescription already exists for this consultation
        if (await _prescriptionRepository.ExistsByConsultationIdAsync(dto.ConsultationId))
        {
            _logger.LogWarning("Prescription already exists for consultation ID: {ConsultationId}", dto.ConsultationId);
            throw new InvalidOperationException($"Prescription already exists for consultation ID {dto.ConsultationId}");
        }

        // Create prescription
        var prescription = new Prescription
        {
            ConsultationId = dto.ConsultationId,
            PrescriptionDate = DateTime.UtcNow,
            Medications = dto.Medications.Select(m => new Medication
            {
                Name = m.Name,
                Dosage = m.Dosage,
                Frequency = m.Frequency,
                Duration = m.Duration,
                Instructions = m.Instructions
            }).ToList()
        };

        var createdPrescription = await _prescriptionRepository.CreateAsync(prescription);

        _logger.LogInformation("Prescription created successfully with ID: {PrescriptionId}", createdPrescription.Id);
        return _mapper.Map<PrescriptionDto>(createdPrescription);
    }

    /// <summary>
    /// Update prescription medications
    /// </summary>
    public async Task<PrescriptionDto> UpdateAsync(int id, UpdatePrescriptionDto dto)
    {
        _logger.LogInformation("Updating prescription with ID: {PrescriptionId}", id);

        var prescription = await _prescriptionRepository.GetByIdAsync(id);
        if (prescription == null)
        {
            _logger.LogWarning("Prescription not found for update with ID: {PrescriptionId}", id);
            throw new ArgumentException($"Prescription with ID {id} not found");
        }

        // Update medications - remove old ones and add new ones
        prescription.Medications.Clear();
        
        foreach (var medDto in dto.Medications)
        {
            var medication = new Medication
            {
                Name = medDto.Name,
                Dosage = medDto.Dosage,
                Frequency = medDto.Frequency,
                Duration = medDto.Duration,
                Instructions = medDto.Instructions,
                PrescriptionId = id
            };
            prescription.Medications.Add(medication);
        }

        var updatedPrescription = await _prescriptionRepository.UpdateAsync(prescription);
        await _prescriptionRepository.SaveChangesAsync();

        _logger.LogInformation("Prescription updated successfully with ID: {PrescriptionId}", id);
        return _mapper.Map<PrescriptionDto>(updatedPrescription);
    }

    /// <summary>
    /// Delete a prescription
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting prescription with ID: {PrescriptionId}", id);

        var prescription = await _prescriptionRepository.GetByIdAsync(id);
        if (prescription == null)
        {
            _logger.LogWarning("Prescription not found for deletion with ID: {PrescriptionId}", id);
            throw new ArgumentException($"Prescription with ID {id} not found");
        }

        await _prescriptionRepository.DeleteAsync(id);
        await _prescriptionRepository.SaveChangesAsync();

        _logger.LogInformation("Prescription deleted successfully with ID: {PrescriptionId}", id);
    }

    /// <summary>
    /// Check if prescription exists for consultation
    /// </summary>
    public async Task<bool> ExistsAsync(int consultationId)
    {
        return await _prescriptionRepository.ExistsByConsultationIdAsync(consultationId);
    }

    /// <summary>
    /// Validate prescription data
    /// </summary>
    public bool ValidatePrescriptionData(CreatePrescriptionDto dto, out List<string> errors)
    {
        errors = new List<string>();

        // Validate consultation ID
        if (dto.ConsultationId <= 0)
        {
            errors.Add("Consultation ID must be greater than 0");
        }

        // Validate medications
        if (dto.Medications == null || dto.Medications.Count == 0)
        {
            errors.Add("At least one medication must be provided");
        }
        else
        {
            foreach (var med in dto.Medications)
            {
                // Validate medication name
                if (string.IsNullOrWhiteSpace(med.Name))
                {
                    errors.Add("Medication name is required");
                }
                else if (med.Name.Length > 255)
                {
                    errors.Add("Medication name must not exceed 255 characters");
                }

                // Validate dosage
                if (string.IsNullOrWhiteSpace(med.Dosage))
                {
                    errors.Add("Dosage is required");
                }
                else if (med.Dosage.Length > 100)
                {
                    errors.Add("Dosage must not exceed 100 characters");
                }

                // Validate frequency
                if (string.IsNullOrWhiteSpace(med.Frequency))
                {
                    errors.Add("Frequency is required");
                }
                else if (med.Frequency.Length > 100)
                {
                    errors.Add("Frequency must not exceed 100 characters");
                }

                // Validate duration
                if (med.Duration < 1 || med.Duration > 365)
                {
                    errors.Add("Duration must be between 1 and 365 days");
                }

                // Validate instructions (optional but has length limit)
                if (!string.IsNullOrWhiteSpace(med.Instructions) && med.Instructions.Length > 500)
                {
                    errors.Add("Instructions must not exceed 500 characters");
                }
            }
        }

        return errors.Count == 0;
    }
}
