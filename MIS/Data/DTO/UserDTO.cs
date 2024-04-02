using System;
using System.ComponentModel.DataAnnotations;

namespace MIS.Data.DTO
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        [Required]
        [MinLength(1)]
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        [Required]
        public string Gender { get; set; }
        [MinLength(1)]
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
    }


}
