using AutoMapper;
using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Models;
using ClinicalPatientManagement.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ClinicalPatientManagement.Api.Services;

/// <summary>
/// Service implementation for patient management operations
/// Step 6: Patient Management - Business Logic Layer with AutoMapper
/// </summary>
public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PatientService(IPatientRepository repository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = Log.ForContext<PatientService>();
    }

    /// <summary>
    /// Get all patients
    /// </summary>
    public async Task<IEnumerable<PatientDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Fetching all patients");
            var patients = await _repository.GetAll().ToListAsync();
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching all patients");
            throw;
        }
    }

    /// <summary>
    /// Get patient by ID
    /// </summary>
    public async Task<PatientDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Fetching patient with ID {PatientId}", id);
            var patient = await _repository.GetByIdAsync(id, cancellationToken);
            return patient == null ? null : _mapper.Map<PatientDto>(patient);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching patient with ID {PatientId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create new patient with validation
    /// </summary>
    public async Task<PatientDto> CreateAsync(CreatePatientDto createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            // Validate
            if (!ValidatePatientData(createDto, out var errors))
            {
                var errorMsg = string.Join("; ", errors);
                _logger.Warning("Patient validation failed: {Errors}", errorMsg);
                throw new InvalidOperationException($"Patient validation failed: {errorMsg}");
            }

            var patient = _mapper.Map<Patient>(createDto);
            var createdPatient = await _repository.AddAsync(patient, cancellationToken);

            _logger.Information("Patient created successfully with ID {PatientId}", createdPatient.Id);
            return _mapper.Map<PatientDto>(createdPatient);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error creating patient");
            throw;
        }
    }

    /// <summary>
    /// Update existing patient with validation
    /// </summary>
    public async Task<PatientDto> UpdateAsync(int id, UpdatePatientDto updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto));

            var patient = await _repository.GetByIdAsync(id, cancellationToken);
            if (patient == null)
                throw new InvalidOperationException($"Patient with ID {id} not found");

            // Validate
            if (!ValidatePatientData(new CreatePatientDto
            {
                FirstName = updateDto.FirstName,
                LastName = updateDto.LastName,
                Phone = updateDto.Phone,
                Email = updateDto.Email,
                DateOfBirth = updateDto.DateOfBirth,
                Gender = updateDto.Gender
            }, out var errors))
            {
                var errorMsg = string.Join("; ", errors);
                _logger.Warning("Patient validation failed for update: {Errors}", errorMsg);
                throw new InvalidOperationException($"Patient validation failed: {errorMsg}");
            }

            var patientToUpdate = _mapper.Map(updateDto, patient);
            var updatedPatient = await _repository.UpdateAsync(patientToUpdate, cancellationToken);

            _logger.Information("Patient with ID {PatientId} updated successfully", id);
            return _mapper.Map<PatientDto>(updatedPatient);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error updating patient with ID {PatientId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete patient by ID
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.DeleteAsync(id, cancellationToken);
            if (result)
            {
                _logger.Information("Patient with ID {PatientId} deleted successfully", id);
            }
            else
            {
                _logger.Warning("Patient with ID {PatientId} not found for deletion", id);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error deleting patient with ID {PatientId}", id);
            throw;
        }
    }

    /// <summary>
    /// Search patients by name or phone (case-insensitive, partial match)
    /// </summary>
    public async Task<IEnumerable<PatientDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Searching patients with term: {SearchTerm}", searchTerm);
            var patients = await _repository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error searching patients with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    /// <summary>
    /// Check if patient exists
    /// </summary>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(id, cancellationToken);
    }

    /// <summary>
    /// Validate patient data
    /// </summary>
    public bool ValidatePatientData(CreatePatientDto dto, out List<string> errors)
    {
        errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.FirstName))
            errors.Add("First name is required");
        else if (dto.FirstName.Length > 100)
            errors.Add("First name cannot exceed 100 characters");

        if (string.IsNullOrWhiteSpace(dto.LastName))
            errors.Add("Last name is required");
        else if (dto.LastName.Length > 100)
            errors.Add("Last name cannot exceed 100 characters");

        if (string.IsNullOrWhiteSpace(dto.Phone))
            errors.Add("Phone is required");
        else if (dto.Phone.Length > 20)
            errors.Add("Phone cannot exceed 20 characters");

        if (!string.IsNullOrEmpty(dto.Email) && !IsValidEmail(dto.Email))
            errors.Add("Email format is invalid");
        else if (dto.Email?.Length > 255)
            errors.Add("Email cannot exceed 255 characters");

        if (dto.DateOfBirth >= DateTime.Now.AddYears(-5))
            errors.Add("Patient must be at least 5 years old");

        if (string.IsNullOrWhiteSpace(dto.Gender) || !IsValidGender(dto.Gender))
            errors.Add("Gender must be Male, Female, or Other");

        return errors.Count == 0;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidGender(string gender)
    {
        return gender.Equals("Male", StringComparison.OrdinalIgnoreCase) ||
               gender.Equals("Female", StringComparison.OrdinalIgnoreCase) ||
               gender.Equals("Other", StringComparison.OrdinalIgnoreCase);
    }
}
