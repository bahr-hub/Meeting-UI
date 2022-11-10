using System;
using System.ComponentModel.DataAnnotations;

namespace Model.DTO
{
    public partial class VacationDto 
    {
        public Guid Id { get; set; }
        [Required]
        public Guid FkUserId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}
