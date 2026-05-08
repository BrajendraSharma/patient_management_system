using AutoMapper;
using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Models;
using ClinicalPatientManagement.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ClinicalPatientManagement.Api.Services;

/// <summary>
/// Service implementation for consultation management operations
/// Step 9: Implement Consultation Creation - Business Logic Layer with AutoMapper
/// </summary>
public class ConsultationService : IConsultationService
{
    private readonly IConsultationRepository _repository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public ConsultationService(IConsultationRepository repository, IAppointmentRepository appointmentRepository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = Log.ForContext<ConsultationService>();
    }

    /// <summary>
    /// Get all consultations
    /// </summary>
    public async Task<IEnumerable<ConsultationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Fetching all consultations");
            var consultations = await _repository.GetAll().ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<ConsultationDto>>(consultations);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching all consultations");
            throw;
        }
    }

    /// <summary>
    /// Get consultation by ID
    /// </summary>
    public async Task<ConsultationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Fetching consultation with ID {ConsultationId}", id);
            var consultation = await _repository.GetByIdAsync(id, cancellationToken);
            return consultation == null ? null : _mapper.Map<ConsultationDto>(consultation);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching consultation with ID {ConsultationId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create new consultation with validation
    /// </summary>
    public async Task<ConsultationDto> CreateAsync(CreateConsultationDto createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            // Validate
            if (!ValidateConsultationData(createDto, out var errors))
            {
                var errorMsg = string.Join("; ", errors);
                _logger.Warning("Consultation validation failed: {Errors}", errorMsg);
                throw new InvalidOperationException($"Consultation validation failed: {errorMsg}");
            }

            // Check if appointment exists
            var appointmentExists = await _appointmentRepository.ExistsAsync(createDto.AppointmentId, cancellationToken);
            if (!appointmentExists)
            {
                _logger.Warning("Appointment with ID {AppointmentId} not found", createDto.AppointmentId);
                throw new InvalidOperationException($"Appointment with ID {createDto.AppointmentId} not found");
            }

            // Check if consultation already exists for this appointment
            var existingConsultation = await _repository.ExistsByAppointmentIdAsync(createDto.AppointmentId, cancellationToken);
            if (existingConsultation)
            {
                _logger.Warning("Consultation already exists for appointment {AppointmentId}", createDto.AppointmentId);
                throw new InvalidOperationException($"Consultation already exists for appointment {createDto.AppointmentId}");
            }

            var consultation = _mapper.Map<Consultation>(createDto);
            var createdConsultation = await _repository.AddAsync(consultation, cancellationToken);

            _logger.Information("Consultation created successfully with ID {ConsultationId} for appointment {AppointmentId}", 
                createdConsultation.Id, createdConsultation.AppointmentId);
            return _mapper.Map<ConsultationDto>(createdConsultation);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error creating consultation");
            throw;
        }
    }

    /// <summary>
    /// Update existing consultation with validation
    /// </summary>
    public async Task<ConsultationDto> UpdateAsync(int id, UpdateConsultationDto updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto));

            var consultation = await _repository.GetByIdAsync(id, cancellationToken);
            if (consultation == null)
                throw new InvalidOperationException($"Consultation with ID {id} not found");

            // Validate
            if (!ValidateConsultationData(new CreateConsultationDto
            {
                AppointmentId = consultation.AppointmentId,
                Temperature = updateDto.Temperature,
                BloodPressure = updateDto.BloodPressure,
                Pulse = updateDto.Pulse,
                Complaints = updateDto.Complaints,
                Diagnosis = updateDto.Diagnosis
            }, out var errors))
            {
                var errorMsg = string.Join("; ", errors);
                _logger.Warning("Consultation validation failed for update: {Errors}", errorMsg);
                throw new InvalidOperationException($"Consultation validation failed: {errorMsg}");
            }

            var consultationToUpdate = _mapper.Map(updateDto, consultation);
            var updatedConsultation = await _repository.UpdateAsync(consultationToUpdate, cancellationToken);

            _logger.Information("Consultation with ID {ConsultationId} updated successfully", id);
            return _mapper.Map<ConsultationDto>(updatedConsultation);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error updating consultation with ID {ConsultationId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete consultation by ID
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.DeleteAsync(id, cancellationToken);
            if (result)
            {
                _logger.Information("Consultation with ID {ConsultationId} deleted successfully", id);
            }
            else
            {
                _logger.Warning("Consultation with ID {ConsultationId} not found for deletion", id);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error deleting consultation with ID {ConsultationId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get consultation by appointment ID
    /// </summary>
    public async Task<ConsultationDto?> GetByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Fetching consultation for appointment {AppointmentId}", appointmentId);
            var consultation = await _repository.GetByAppointmentIdAsync(appointmentId, cancellationToken);
            return consultation == null ? null : _mapper.Map<ConsultationDto>(consultation);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching consultation for appointment {AppointmentId}", appointmentId);
            throw;
        }
    }

    /// <summary>
    /// Get all consultations for a patient
    /// </summary>
    public async Task<IEnumerable<ConsultationDto>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Fetching consultations for patient {PatientId}", patientId);
            var consultations = await _repository.GetByPatientIdAsync(patientId, cancellationToken);
            return _mapper.Map<IEnumerable<ConsultationDto>>(consultations);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching consultations for patient {PatientId}", patientId);
            throw;
        }
    }

    /// <summary>
    /// Check if consultation exists
    /// </summary>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(id, cancellationToken);
    }

    /// <summary>
    /// Validate consultation data
    /// </summary>
    public bool ValidateConsultationData(CreateConsultationDto dto, out List<string> errors)
    {
        errors = new List<string>();

        if (dto.AppointmentId <= 0)
            errors.Add("Appointment ID must be greater than 0");

        if (dto.Temperature < 30 || dto.Temperature > 45)
            errors.Add("Temperature must be between 30 and 45 degrees Celsius");

        if (string.IsNullOrWhiteSpace(dto.BloodPressure))
            errors.Add("Blood pressure is required");
        else if (!System.Text.RegularExpressions.Regex.IsMatch(dto.BloodPressure, @"^\d{2,3}/\d{2,3}$"))
            errors.Add("Blood pressure must be in format XXX/XXX (e.g., 120/80)");

        if (dto.Pulse < 40 || dto.Pulse > 200)
            errors.Add("Pulse must be between 40 and 200 bpm");

        if (string.IsNullOrWhiteSpace(dto.Complaints))
            errors.Add("Complaints are required");
        else if (dto.Complaints.Length > 1000)
            errors.Add("Complaints cannot exceed 1000 characters");

        if (string.IsNullOrWhiteSpace(dto.Diagnosis))
            errors.Add("Diagnosis is required");
        else if (dto.Diagnosis.Length > 1000)
            errors.Add("Diagnosis cannot exceed 1000 characters");

        return errors.Count == 0;
    }
}
