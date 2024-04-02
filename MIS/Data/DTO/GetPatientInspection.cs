using System.ComponentModel.DataAnnotations;

namespace MIS.Data.DTO
{
    public class GetPatientInspection
    {
        public bool? grouped { get; set; }

        public Guid[]? icdRoots { get; set; }
        public int? size { get; set; }
        public int? page { get; set; }
    }
}
