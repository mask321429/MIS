using System;
using System.ComponentModel.DataAnnotations;

namespace MIS.Data.Models
{
    public class SpecialityGetModel
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }

    }
}
