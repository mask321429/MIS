using MIS.Data.Models;

namespace MIS.Data.DTO
{
    public class FullInfoAboutInspectionDTO
    {

        public DateTime Date { get; set; }
        public string Anamnesis { get; set; }
        public string Complaints { get; set; }
        public string Treatment { get; set; }
        public Conclusion Conclusion { get; set; }
        public DateTime? NextVisitDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public Guid? BaseInspectionId { get; set; }
        public Guid? PreviousInspectionId { get; set; }
        public PatientDTOFull Patient { get; set; }
        public DoctorDTOFull Doctor { get; set; }
        public List<DiagnosisDTOFull> Diagnoses { get; set; }
        public List<ConsultationDTOFull> Consultations { get; set; }
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class PatientDTOFull
    {
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class DoctorDTOFull
    {
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class DiagnosisDTOFull
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DiagnosisType Type { get; set; }
        public Guid Id { get; set; }
        public Guid InspectionId { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class ConsultationDTOFull
    {
        public Guid InspectionId { get; set; }
        public SpecialityDTO Speciality { get; set; }
        public RootCommentDTO? RootComment { get; set; }
        public int CommentsNumber { get; set; }
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid? SpecialityId { get; internal set; }
        public Guid? CommentId { get; internal set; }
    }

    public class SpecialityDTOFull
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class RootCommentDTO
    {
        public Guid? ParentId { get; set; }
        public string Content { get; set; }
        public AuthorDTO Author { get; set; }
        public DateTime ModifyTime { get; set; }
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class AuthorDTO
    {
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
    }

}

