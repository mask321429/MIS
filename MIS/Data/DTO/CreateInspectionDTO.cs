using MIS.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MIS.Data.DTO
{
    public class CreateInspectionInfoDTO
    {
        public DateTime Date { get; set; }
        public string Anamnesis { get; set; }
        public string Complaints { get; set; }
        public string Treatment { get; set; }
        public Conclusion Conclusion { get; set; }
        public DateTime NextVisitDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public Guid PreviousInspectionId { get; set; }
        public List<DiagnosisDTO> Diagnoses { get; set; }
        public List<ConsultationDTO> Consultations { get; set; }
    }

    public class DiagnosisDTO
    {
        public Guid IcdDiagnosisId { get; set; }
        public string Description { get; set; }
        public DiagnosisType Type { get; set; }
    }

    public class ConsultationDTO
    {
        public Guid SpecialityId { get; set; }
        public CommentDTO Comment { get; set; }
    }

    public class CommentDTO
    {
        [MinLength(1)]
        public string content { get; set; }
    }

    public class CreateInspectionDTO
    {
        public CreateInspectionInfoDTO CreateInspectionInfo { get; set; }
    }
}
