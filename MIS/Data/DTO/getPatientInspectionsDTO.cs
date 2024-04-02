using MIS.Data.Models;

namespace MIS.Data.DTO
{
    public class getPatientInspectionsDTO
    {
        public Guid Id { get; set; }
        public DateTime createTime { get; set; }
        public Guid previousId { get; set; }
        public DateTime date { get; set; }
        public Conclusion conclusion { get; set; }
        public Guid doctorId { get; set; }
        public string doctor { get; set; }
        public Guid patientId { get; set; }
        public string patient { get; set;}
        public  DiagnosisModel diagnosis { get; set; }
        public bool hasNasted { get; set; }
        public bool hasChain { get; set; }
    }

    public class InspectionsResponseDTO
    {
        public List<getPatientInspectionsDTO> Inspections { get; set; }
        public PaginationDTO Pagination { get; set; }
    }
}
