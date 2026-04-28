using AutoMapper;

namespace ClinicalPatientManagement.Api.Mappings;

/// <summary>
/// AutoMapper profile configuration for entity-to-DTO mappings
/// Will be expanded with specific entity mappings in later steps
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity to DTO mappings will be added here
        // Pattern: CreateMap<Entity, EntityDTO>().ReverseMap();
        
        // Example (to be added in Step 6):
        // CreateMap<Patient, PatientDto>().ReverseMap();
        // CreateMap<Appointment, AppointmentDto>().ReverseMap();
        // CreateMap<Consultation, ConsultationDto>().ReverseMap();
    }
}
