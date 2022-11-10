using Model.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Model.DTO
{
    public partial class RoleDto 
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public virtual ICollection<RoleModulePrivilegeDto> RoleModulePrivilege { get; set; }
    }
    public class RoleChangeStatus
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
