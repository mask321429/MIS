using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MIS.Data.Models
{
    public class Inspection
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime Date { get; set; }
        public string? Anamnesis { get; set; }
        public string? Complaints { get; set; }
        public string? Treatment { get; set; }
        public Conclusion? Conclusion { get; set; }
        public DateTime? NextVisitDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public Guid? BaseInspectionId { get; set; }
        public Guid? PreviousInspectionId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public List<DiagnosisModel>? Diagnoses { get; set; }
        public List<ConsultationModel>? Consultations { get; set; }
        public bool hasNested { get; set; }
        public bool hasChain { get; set; }
    }

   
    public class DiagnosisModel
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DiagnosisType Type { get; set; }
        public Guid InspectionId { get; set; }
        public Guid IdDiagnosis { get; set; }
        public Guid IdPatient { get; set; }
        public Guid IdDoctor { get; set; }
    }

    public class ConsultationModel
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid? InspectionId { get; set; }
        public Guid? SpecialityId { get; set; }
        public Guid? CommentId { get; set; }     
    }

    public class InspectionCommentModel
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid? Parent { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public Guid AuthorID { get; set; }
        public DateTime? ModifyTime { get; set; }
        public Guid IdConsultation { get; set; }
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Conclusion
    {
        Disease,
        Recovery,
        Death
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DiagnosisType
    {
        Main,
        Concomitant,
        Complication
    }

   public class PatientDiagnosis
    {
        [Key]
        public Guid id { get; set; }
        public Guid idPatient { get; set; }
        public Guid idDiagnosis { get; set; }
    }


    public class InspectionPatient
    {
        [Key]
        public Guid id { get; set; }
        public Guid idPatient { get; set; }
        public Guid idInspection { get; set; }
    }

    public class InspectionDoctor
    {
        [Key]
        public Guid id { get; set; }
        public Guid idDoctor { get; set; }
        public Guid idInspection { get; set; }
    }



}
