using MIS.Data.Models;
using System.Security.Cryptography.X509Certificates;

namespace MIS.Data.DTO
{
    public class GetPatientDTO
    {
        public string? Name { get; set; }
        public List<Conclusion>? Conclusions { get; set; }
        public Sorting? Sortings { get; set; }
        public bool? ScheduledVisits { get; set; }
        public bool? OnlyMine { get; set; }
        public int? Page { get; set; }
        public int? Size { get; set; }
    }

    public enum Sorting
    {
        NameAsc,
        NameDesc,
        CreateAsc,
        CreateDesc,
        InspectionAsc,
        InspectionDesc
    }

    public class ReturnDTOGetPatient
    {
        public List<PatientModel> patientModels { get; set; }
        public PaginationDTO Pagination { get; set; }
    }
}
