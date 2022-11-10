using System;
using System.ComponentModel.DataAnnotations;

namespace Model.DTO
{
    public partial class LocationDto 
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string NameAr { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
    public class LocationChangeStatus
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
