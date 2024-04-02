
namespace MIS.Data.Models
{
    public class Record
    {
        public Guid ID { get; set; }
        public int ACTUAL { get; set; }
        public string MKB_CODE { get; set; }
        public string MKB_NAME { get; set; }
        public string REC_CODE { get; set; }
        public Guid? ID_PARENT { get; set; }
        public DateTime  dateTime { get; set; }
        public Guid ROOT { get; set; }
    }

    public class RootObject
    {
        public List<Record> records { get; set; }
    }
}
