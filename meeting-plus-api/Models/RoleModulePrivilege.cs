using System;
using System.Collections.Generic;

namespace MeetingPlus.API.Models
{
    public partial class RoleModulePrivilege
    {
        public int FkPrivilegeId { get; set; }
        public int FkModuleId { get; set; }
        public int FkRoleId { get; set; }

        public virtual Module FkModule { get; set; }
        public virtual Privilege FkPrivilege { get; set; }
        public virtual Role FkRole { get; set; }
    }
}
