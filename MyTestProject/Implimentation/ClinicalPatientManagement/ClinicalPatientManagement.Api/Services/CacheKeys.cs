namespace ClinicalPatientManagement.Api.Services
{
    public static class CacheKeys
    {
        public static string Patient(int id) => $"patient:{id}";
        public static string AllPatients => "patients:all";
        public static string Appointment(int id) => $"appointment:{id}";
        public static string AllAppointments => "appointments:all";
        public static string Consultation(int id) => $"consultation:{id}";
        public static string AllConsultations => "consultations:all";
    }
}
