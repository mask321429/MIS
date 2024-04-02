using MIS.Data.Models;

namespace MIS.Data.DTO
{
    public class GetInspectionCardDTO
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public DateTime data { get; set; }
        public DiagnosisModel diagnosis { get; set; }
    }
}
