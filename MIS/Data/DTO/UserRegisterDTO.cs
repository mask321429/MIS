using System;
using System.ComponentModel.DataAnnotations;

namespace MIS.Data.DTO
{
    public class UserRegisterDTO
    {
        [Required]
        [MinLength(1)]
        public string? FullName { get; set; }
        [Required]
        [MinLength(6)]
        public string? Password { get; set; }
        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        [Required]
        public string? Gender { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public Guid SpecialityId { get; set; }
    }


}
