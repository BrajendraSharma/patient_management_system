using AutoMapper;
using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Models;
using ClinicalPatientManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ClinicalPatientManagement.Api.Services;

/// <summary>
/// Service implementation for appointment management operations
/// Step 7: Appointment Scheduling - Business Logic Layer with AutoMapper
/// Phase 2: Updated to use IUnitOfWork pattern for transaction management
/// Phase 3: Caching support for read-heavy operations
/// </summary>
public class AppointmentService : IAppointmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly ICacheService _cacheService;

    public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = Log.ForContext<AppointmentService>();
    }

    /// <summary>
    /// Get all appointments with caching support (5-minute TTL)
    /// Phase 3: Performance Optimization - Caching for read-heavy operations
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = CacheKeys.AllAppointments;
            
            // Try to get from cache
            var cachedAppointments = await _cacheService.GetAsync<IEnumerable<AppointmentDto>>(cacheKey, cancellationToken);
            if (cachedAppointments != null)
            {
                _logger.Information("Retrieved appointments from cache");
                return cachedAppointments;
            }

            _logger.Information("Fetching all appointments from database");
            var appointments = await _unitOfWork.Appointments.GetAll().ToListAsync(cancellationToken);
            var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            
            // Cache for 5 minutes
            await _cacheService.SetAsync(cacheKey, appointmentDtos, TimeSpan.FromMinutes(5), cancellationToken);
            
            return appointmentDtos;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching all appointments");
            throw;
        }
    }

    /// <summary>
    /// Get appointment by ID with caching support (5-minute TTL)
    /// Phase 3: Performance Optimization - Caching for read-heavy operations
    /// </summary>
    public async Task<AppointmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = CacheKeys.Appointment(id);
            
            // Try to get from cache
            var cachedAppointment = await _cacheService.GetAsync<AppointmentDto>(cacheKey, cancellationToken);
            if (cachedAppointment != null)
            {
                _logger.Information("Retrieved appointment {AppointmentId} from cache", id);
                return cachedAppointment;
            }

            _logger.Information("Fetching appointment with ID {AppointmentId} from database", id);
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id, cancellationToken);
            if (appointment == null)
                return null;

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
            
            // Cache for 5 minutes
            await _cacheService.SetAsync(cacheKey, appointmentDto, TimeSpan.FromMinutes(5), cancellationToken);
            
            return appointmentDto;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching appointment with ID {AppointmentId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create new appointment with validation and conflict checking
    /// Phase 2: Uses UnitOfWork to persist changes (ensures ACID compliance)
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
            if (!await _unitOfWork.Patients.ExistsAsync(createDto.PatientId, cancellationToken))
            {
                var errorMsg = $"Patient with ID {createDto.PatientId} not found";
                _logger.Warning(errorMsg);
                throw new InvalidOperationException(errorMsg);
            }

            // Check for appointment conflicts
            if (await _unitOfWork.Appointments.HasConflictAsync(createDto.PatientId, createDto.AppointmentDate, cancellationToken))
            {
                var errorMsg = $"Patient already has an appointment scheduled within 30 minutes of {createDto.AppointmentDate}";
                _logger.Warning(errorMsg);
                throw new InvalidOperationException(errorMsg);
            }

            var appointment = _mapper.Map<Appointment>(createDto);
            var createdAppointment = await _unitOfWork.Appointments.AddAsync(appointment, cancellationToken);
            
            // Persist changes through UnitOfWork
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Phase 3: Invalidate cache on create
            await _cacheService.RemoveAsync(CacheKeys.AllAppointments, cancellationToken);

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
    /// Phase 2: Uses UnitOfWork to persist changes (ensures ACID compliance)
    /// </summary>
    public async Task<AppointmentDto> UpdateAsync(int id, UpdateAppointmentDto updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto));

            var existingAppointment = await _unitOfWork.Appointments.GetByIdAsync(id, cancellationToken);
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
                if (await _unitOfWork.Appointments.HasConflictAsync(updateDto.PatientId, updateDto.AppointmentDate, cancellationToken))
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

            var updatedAppointment = await _unitOfWork.Appointments.UpdateAsync(existingAppointment, cancellationToken);
            
            // Persist changes through UnitOfWork
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Phase 3: Invalidate cache on update
            await _cacheService.RemoveAsync(CacheKeys.AllAppointments, cancellationToken);
            await _cacheService.RemoveAsync(CacheKeys.Appointment(id), cancellationToken);
            
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
    /// Phase 2: Uses UnitOfWork to persist changes (ensures ACID compliance)
    /// Phase 3: Invalidates cache on delete
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Deleting appointment with ID {AppointmentId}", id);
            var result = await _unitOfWork.Appointments.DeleteAsync(id, cancellationToken);
            if (result)
            {
                // Persist changes through UnitOfWork
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Phase 3: Invalidate cache on delete
                await _cacheService.RemoveAsync(CacheKeys.AllAppointments, cancellationToken);
                await _cacheService.RemoveAsync(CacheKeys.Appointment(id), cancellationToken);

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
            var appointments = await _unitOfWork.Appointments.GetByPatientId(patientId).ToListAsync(cancellationToken);
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
            var appointments = await _unitOfWork.Appointments.GetByPatientIdAndDateRangeAsync(patientId, startDate, endDate, cancellationToken);
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
            var appointments = await _unitOfWork.Appointments.GetByDateAsync(date, cancellationToken);
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
            var appointments = await _unitOfWork.Appointments.GetByDateAndStatusAsync(date, status, cancellationToken);
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
            var appointment = await _unitOfWork.Appointments.UpdateStatusAsync(id, status, cancellationToken);
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
            var appointments = await _unitOfWork.Appointments.GetUpcomingAppointmentsAsync(patientId, cancellationToken);
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
            return await _unitOfWork.Appointments.HasConflictAsync(patientId, appointmentDate, cancellationToken);
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
        return await _unitOfWork.Appointments.ExistsAsync(id, cancellationToken);
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
