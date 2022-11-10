using Model.Models;
using System.Collections.Generic;

namespace Model.DTO
{
    public partial class RoleModulePrivilegeDto
    {
        public int FkPrivilegeId { get; set; }
        public int FkModuleId { get; set; }
        public int FkRoleId { get; set; }
    }
}
