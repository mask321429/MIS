namespace MIS.Data.DTO
{
    public class DTOGetReport
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Guid[] icdRoot { get; set; }
    }
}
