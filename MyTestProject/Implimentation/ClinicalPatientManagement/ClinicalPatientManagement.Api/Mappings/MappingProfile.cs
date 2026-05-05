using AutoMapper;
using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Models;

namespace ClinicalPatientManagement.Api.Mappings;

/// <summary>
/// AutoMapper profile configuration for entity-to-DTO mappings
/// Step 6: Patient Management - Maps Patient entities to DTOs
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Step 6: Patient Management mappings
        CreateMap<Patient, PatientDto>().ReverseMap();
        CreateMap<CreatePatientDto, Patient>();
        CreateMap<UpdatePatientDto, Patient>();
        
        // Future steps will add more mappings here
        // CreateMap<Appointment, AppointmentDto>().ReverseMap();
        // CreateMap<Consultation, ConsultationDto>().ReverseMap();
    }
}
