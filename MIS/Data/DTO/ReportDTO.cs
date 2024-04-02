namespace MIS.Data.DTO
{
    public class ResponseDTO
    {
        public FilterData Filters { get; set; }
        public List<RecordData> Records { get; set; }
        public Dictionary<string, int> SummaryByRoot { get; set; }
    }

    public class FilterData
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<string> IcdRoots { get; set; }
    }

    public class RecordData
    {
        public string PatientName { get; set; }
        public DateTime PatientBirthdate { get; set; }
        public string Gender { get; set; }
        public Dictionary<string, int> VisitsByRoot { get; set; }
    }

  

}
