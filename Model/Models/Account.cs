using System;
using System.Collections.Generic;

namespace Model.Models
{
    public partial class Account
    {
        public Account()
        {
            InverseFkParentAccount = new HashSet<Account>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? FkParentAccountId { get; set; }
        public Guid? FkAdminId { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string Photo { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Account FkParentAccount { get; set; }
        public virtual ICollection<Account> InverseFkParentAccount { get; set; }
    }
}
