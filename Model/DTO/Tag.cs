using System;
using System.ComponentModel.DataAnnotations;

namespace Model.DTO
{
    public partial class TagDto 
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
    public class TagChangeStatus
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
