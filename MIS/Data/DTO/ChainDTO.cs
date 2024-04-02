using MIS.Data.Models;

namespace MIS.Data.DTO
{
    public class ChainDTO
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid? PreviousId { get; set; }
        public DateTime Date { get; set; }
        public Conclusion Conclusion { get; set; }
        public Guid DoctorId { get; set; }
        public string Doctor { get; set; }
        public Guid PatientId { get; set; }
        public string Patient { get; set; }
        public List<DiagnosisForDTO> Diagnosis { get; set; }
        public bool HasChain { get; set; }
        public bool HasNested { get; set; }
    }

    public class DiagnosisForDTO
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DiagnosisType Type { get; set; }
    }
}
