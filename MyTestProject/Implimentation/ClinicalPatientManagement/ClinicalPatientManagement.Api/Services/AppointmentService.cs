using AutoMapper;
using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Models;
using ClinicalPatientManagement.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ClinicalPatientManagement.Api.Services;

/// <summary>
/// Service implementation for appointment management operations
/// Step 7: Appointment Scheduling - Business Logic Layer with AutoMapper
/// </summary>
public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _repository;
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public AppointmentService(IAppointmentRepository repository, IPatientRepository patientRepository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = Log.ForContext<AppointmentService>();
    }

    /// <summary>
    /// Get all appointments
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Fetching all appointments");
            var appointments = await _repository.GetAll().ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching all appointments");
            throw;
        }
    }

    /// <summary>
    /// Get appointment by ID
    /// </summary>
    public async Task<AppointmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Fetching appointment with ID {AppointmentId}", id);
            var appointment = await _repository.GetByIdAsync(id, cancellationToken);
            return appointment == null ? null : _mapper.Map<AppointmentDto>(appointment);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching appointment with ID {AppointmentId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create new appointment with validation and conflict checking
    /// </summary>
    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            // Validate appointment data
            if (!ValidateAppointmentData(createDto, out var errors))
            {
                var errorMsg = string.Join("; ", errors);
                _logger.Warning("Appointment validation failed: {Errors}", errorMsg);
                throw new InvalidOperationException($"Appointment validation failed: {errorMsg}");
            }

            // Check if patient exists
            if (!await _patientRepository.ExistsAsync(createDto.PatientId, cancellationToken))
            {
                var errorMsg = $"Patient with ID {createDto.PatientId} not found";
                _logger.Warning(errorMsg);
                throw new InvalidOperationException(errorMsg);
            }

            // Check for appointment conflicts
            if (await _repository.HasConflictAsync(createDto.PatientId, createDto.AppointmentDate, cancellationToken))
            {
                var errorMsg = $"Patient already has an appointment scheduled within 30 minutes of {createDto.AppointmentDate}";
                _logger.Warning(errorMsg);
                throw new InvalidOperationException(errorMsg);
            }

            var appointment = _mapper.Map<Appointment>(createDto);
            var createdAppointment = await _repository.AddAsync(appointment, cancellationToken);

            _logger.Information("Appointment created successfully with ID {AppointmentId} for patient {PatientId}", createdAppointment.Id, createDto.PatientId);
            return _mapper.Map<AppointmentDto>(createdAppointment);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error creating appointment");
            throw;
        }
    }

    /// <summary>
    /// Update existing appointment with validation
    /// </summary>
    public async Task<AppointmentDto> UpdateAsync(int id, UpdateAppointmentDto updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto));

            var existingAppointment = await _repository.GetByIdAsync(id, cancellationToken);
            if (existingAppointment == null)
            {
                _logger.Warning("Appointment with ID {AppointmentId} not found", id);
                throw new InvalidOperationException($"Appointment with ID {id} not found");
            }

            // Validate appointment data
            var createDto = new CreateAppointmentDto
            {
                PatientId = updateDto.PatientId,
                AppointmentDate = updateDto.AppointmentDate,
                Status = updateDto.Status,
                Notes = updateDto.Notes
            };

            if (!ValidateAppointmentData(createDto, out var errors))
            {
                var errorMsg = string.Join("; ", errors);
                _logger.Warning("Appointment validation failed: {Errors}", errorMsg);
                throw new InvalidOperationException($"Appointment validation failed: {errorMsg}");
            }

            // Check for appointment conflicts (excluding current appointment)
            if (existingAppointment.PatientId != updateDto.PatientId || existingAppointment.AppointmentDate != updateDto.AppointmentDate)
            {
                if (await _repository.HasConflictAsync(updateDto.PatientId, updateDto.AppointmentDate, cancellationToken))
                {
                    var errorMsg = $"Patient already has an appointment scheduled within 30 minutes of {updateDto.AppointmentDate}";
                    _logger.Warning(errorMsg);
                    throw new InvalidOperationException(errorMsg);
                }
            }

            existingAppointment.PatientId = updateDto.PatientId;
            existingAppointment.AppointmentDate = updateDto.AppointmentDate;
            existingAppointment.Status = updateDto.Status;
            existingAppointment.Notes = updateDto.Notes;

            var updatedAppointment = await _repository.UpdateAsync(existingAppointment, cancellationToken);
            _logger.Information("Appointment {AppointmentId} updated successfully", id);
            return _mapper.Map<AppointmentDto>(updatedAppointment);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error updating appointment with ID {AppointmentId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete appointment by ID
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Deleting appointment with ID {AppointmentId}", id);
            var result = await _repository.DeleteAsync(id, cancellationToken);
            if (result)
            {
                _logger.Information("Appointment {AppointmentId} deleted successfully", id);
            }
            else
            {
                _logger.Warning("Appointment with ID {AppointmentId} not found for deletion", id);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error deleting appointment with ID {AppointmentId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get appointments for a specific patient
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Fetching appointments for patient {PatientId}", patientId);
            var appointments = await _repository.GetByPatientId(patientId).ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching appointments for patient {PatientId}", patientId);
            throw;
        }
    }

    /// <summary>
    /// Get appointments by date range for a patient
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetByPatientIdAndDateRangeAsync(int patientId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Fetching appointments for patient {PatientId} between {StartDate} and {EndDate}", patientId, startDate, endDate);
            var appointments = await _repository.GetByPatientIdAndDateRangeAsync(patientId, startDate, endDate, cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching appointments for patient {PatientId}", patientId);
            throw;
        }
    }

    /// <summary>
    /// Get appointments scheduled for a specific date
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Fetching appointments for date {Date}", date.Date);
            var appointments = await _repository.GetByDateAsync(date, cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching appointments for date {Date}", date.Date);
            throw;
        }
    }

    /// <summary>
    /// Get appointments with specific status on a given date
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetByDateAndStatusAsync(DateTime date, string status, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Fetching appointments with status {Status} for date {Date}", status, date.Date);
            var appointments = await _repository.GetByDateAndStatusAsync(date, status, cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching appointments for date {Date} with status {Status}", date.Date, status);
            throw;
        }
    }

    /// <summary>
    /// Update appointment status
    /// </summary>
    public async Task<AppointmentDto> UpdateStatusAsync(int id, string status, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Updating appointment {AppointmentId} status to {Status}", id, status);
            var appointment = await _repository.UpdateStatusAsync(id, status, cancellationToken);
            _logger.Information("Appointment {AppointmentId} status updated to {Status}", id, status);
            return _mapper.Map<AppointmentDto>(appointment);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error updating appointment status");
            throw;
        }
    }

    /// <summary>
    /// Get upcoming appointments for a patient
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetUpcomingAppointmentsAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Fetching upcoming appointments for patient {PatientId}", patientId);
            var appointments = await _repository.GetUpcomingAppointmentsAsync(patientId, cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching upcoming appointments for patient {PatientId}", patientId);
            throw;
        }
    }

    /// <summary>
    /// Check if patient has appointment conflict
    /// </summary>
    public async Task<bool> HasConflictAsync(int patientId, DateTime appointmentDate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Checking for appointment conflicts for patient {PatientId} at {AppointmentDate}", patientId, appointmentDate);
            return await _repository.HasConflictAsync(patientId, appointmentDate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error checking appointment conflicts");
            throw;
        }
    }

    /// <summary>
    /// Check if appointment exists
    /// </summary>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(id, cancellationToken);
    }

    /// <summary>
    /// Validate appointment data with enhanced checks
    /// </summary>
    public bool ValidateAppointmentData(CreateAppointmentDto dto, out List<string> errors)
    {
        errors = new List<string>();

        if (dto == null)
        {
            errors.Add("Appointment data cannot be null");
            return false;
        }

        // Patient ID validation
        if (dto.PatientId <= 0)
            errors.Add("Patient ID must be greater than 0");

        // Appointment date validation
        if (dto.AppointmentDate == default)
            errors.Add("Appointment date is required");
        else if (dto.AppointmentDate < DateTime.UtcNow)
            errors.Add("Appointment date cannot be in the past");
        else if (dto.AppointmentDate > DateTime.UtcNow.AddYears(1))
            errors.Add("Appointment cannot be scheduled more than 1 year in advance");

        // Status validation
        if (string.IsNullOrWhiteSpace(dto.Status))
            errors.Add("Status is required");
        else
        {
            var validStatuses = new[] { "Scheduled", "Completed", "Cancelled", "No-Show" };
            if (!validStatuses.Contains(dto.Status))
                errors.Add($"Status must be one of: {string.Join(", ", validStatuses)}");
        }

        // Notes validation (optional)
        if (!string.IsNullOrEmpty(dto.Notes) && dto.Notes.Length > 1000)
            errors.Add("Notes cannot exceed 1000 characters");

        return errors.Count == 0;
    }
}
