using System;
using System.Collections.Generic;

namespace Model.Models
{
    public partial class UserRole
    {
        public Guid FkUserId { get; set; }
        public int FkRoleId { get; set; }

        public virtual Role FkRole { get; set; }
        public virtual User FkUser { get; set; }
    }
}
