using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Model.Models
{
    public partial class BaseDataBaseContext
    {
        public IHttpContextAccessor contextAccessor;

        public override int SaveChanges()
        {
            this.AuditEntities();

            return base.SaveChanges();
        }

        private void AuditEntities()
        {

            // Get the authenticated user name 
            Guid? UserID = null;

            if (contextAccessor != null)
            {
                var user = (contextAccessor.HttpContext==null)?null : contextAccessor.HttpContext.User;
                if (user != null)
                {
                    var identity = user.Identity;
                    if (identity != null)
                    {
                        if (!string.IsNullOrEmpty(identity.Name))
                            UserID = Guid.Parse(identity.Name);
                    }
                }
            }
            // Get current date & time
            DateTime now = DateTime.UtcNow;

            // For every changed entity marked as IAditable set the values for the audit properties
            foreach (EntityEntry entry in ChangeTracker.Entries())
            {
                // If the entity was added.
                if (entry.State == EntityState.Added)
                {
                    if (entry.Properties.FirstOrDefault(x => x.Metadata.Name == "Id") != null)
                    {
                        var IDValue = new Guid();
                        bool IsParsed = Guid.TryParse(entry.Property("Id").OriginalValue.ToString(), out IDValue);
                        //if (IsParsed)
                        //{
                        //    entry.Property("Id").CurrentValue = Guid.NewGuid();
                        //}
                        if (IsParsed)
                        {
                            if (IDValue == Guid.Empty)
                                entry.Property("Id").CurrentValue = Guid.NewGuid();
                        }
                }
                    if (entry.Properties.FirstOrDefault(x => x.Metadata.Name == "CreatedBy") != null)
                    {
                        entry.Property("CreatedBy").CurrentValue = UserID;
                    }
                    if (entry.Properties.FirstOrDefault(x => x.Metadata.Name == "CreatedAt") != null)
                    {
                        entry.Property("CreatedAt").CurrentValue = now;
                    }
                    var DateTimeProperties = entry.Properties.Where(x => x.Metadata.PropertyInfo.PropertyType 
                    == typeof(DateTime)).ToList();
                    foreach(var e in DateTimeProperties)
                    {
                        if (e.OriginalValue != e.CurrentValue)
                        {
                            var DateTimeUTC = TimeZoneInfo.ConvertTimeBySystemTimeZoneId((DateTime)e.CurrentValue, "UTC");
                            e.CurrentValue = DateTimeUTC;
                        }
                    }

                }
                else if (entry.State == EntityState.Modified) // If the entity was updated
                {
                    if (entry.Properties.FirstOrDefault(x => x.Metadata.Name == "UpdatedBy") != null)
                    {
                        entry.Property("UpdatedBy").CurrentValue = UserID;
                    }
                    if (entry.Properties.FirstOrDefault(x => x.Metadata.Name == "UpdatedAt") != null)
                    {
                        entry.Property("UpdatedAt").CurrentValue = now;
                    }
                    var DateTimeProperties = entry.Properties.Where(x => x.Metadata.PropertyInfo.PropertyType
                     == typeof(DateTime)).ToList();
                    foreach (var e in DateTimeProperties)
                    {
                        if (!e.OriginalValue.Equals(e.CurrentValue))
                        {
                            var DateTimeUTC = TimeZoneInfo.ConvertTimeBySystemTimeZoneId((DateTime)e.CurrentValue, "UTC");
                            e.CurrentValue = DateTimeUTC;
                        }
                    }
                }
            }
        }
    }
}
