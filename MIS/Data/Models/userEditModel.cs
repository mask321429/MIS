using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MIS.Data.Models
{
    public class userEditModel
    {
        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string? Email { get; set; }     
        public string? Name { get; set; }
        public DateTime? BirthDate { get; set; }
        [Required]
        public string? Gender { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
