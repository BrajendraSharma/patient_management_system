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
        
        // Step 7: Appointment Scheduling mappings
        CreateMap<Appointment, AppointmentDto>().ReverseMap();
        CreateMap<CreateAppointmentDto, Appointment>();
        CreateMap<UpdateAppointmentDto, Appointment>();
        
        // Step 9: Consultation Creation mappings
        CreateMap<Consultation, ConsultationDto>().ReverseMap();
        CreateMap<CreateConsultationDto, Consultation>();
        CreateMap<UpdateConsultationDto, Consultation>();
    }
}
