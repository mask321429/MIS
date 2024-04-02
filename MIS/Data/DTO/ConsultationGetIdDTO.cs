namespace MIS.Data.DTO
{
    public class ConsultationForGetIdDTO
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid InspectionId { get; set; }
        public SpecialityDTO Speciality { get; set; }
        public List<CommentGetIdDTO> Comments { get; set; }
    }


    public class CommentGetIdDTO
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Content { get; set; }
        public Guid AuthorId { get; set; }
        public string Author { get; set; }
        public Guid? ParentId { get; set; }
    }
}
