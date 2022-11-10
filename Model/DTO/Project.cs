using System;
using System.ComponentModel.DataAnnotations;

namespace Model.DTO
{
    public partial class ProjectDto 
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        //[Required]
        //public string NameAr { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
    public class ProjectChangeStatus
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
