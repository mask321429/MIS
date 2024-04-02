using System;
using System.ComponentModel.DataAnnotations;

namespace MIS.Data.DTO
{
    public class SpecialityGetDTO
    {
        public string? Name { get; set; }
        public int? Page { get; set; }
        public int? Size { get; set; }
    }

    public class PaginationDTO
    {
        public int? Size { get; set; }
        public int? Count { get; set; }
        public int? Current { get; set; }
    }

    public class SpecialityDTO
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
    }
    public class SpecialityWithCodeDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class SpecialitiesResponseDTO
    {
        public List<SpecialityDTO> Specialties { get; set; }
        public PaginationDTO Pagination { get; set; }
    }
    public class SpecialitiesNameAndIdDTO
    {
        public List<SpecialityWithCodeDTO> Specialties { get; set; }
        public PaginationDTO Pagination { get; set; }
    }
}
