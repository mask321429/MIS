namespace MIS.Data.Models
{
    public class PatientModel
    {
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public string Gender { get; set; }
        public Guid Id { get; set; }
        public DateTime BirthDate { get; set; }
        public ICollection<Inspection> Inspections { get; set; }
    }

    
}
