using MIS.Data.Models;

namespace MIS.Data.DTO
{
    public class UpdateInspectionDTO
    {
        public string Anamnesis { get; set; }
        public string Complaints { get; set; }
        public string Treatment { get; set; }
        public Conclusion? Conclusion { get; set; }
        public DateTime? NextVisitDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public List<UpdateDiagnosisDTO> Diagnoses { get; set; }
    }

    public class UpdateDiagnosisDTO
    {
        public Guid IcdDiagnosisId { get; set; }
        public string Description { get; set; }
        public DiagnosisType Type { get; set; }
    }
}
