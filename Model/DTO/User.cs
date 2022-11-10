using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Model.Models;

namespace Model.DTO
{
    public class UserDto
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Mobile { get; set; }
        public string Password { get; set; }
        public Guid? FkUserProfileId { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }

        public string Token { get; set; }
        public string Photo { get; set; }
        public string ImageBase64 { get; set; }
        public string ImageUrl { get; set; }
        public int? LocationID { get; set; }

        public ICollection<VacationDto> VacationFkUser { get; set; }
        public ICollection<UserRoleDto> UserRole { get; set; }
        [Required]
        public UserProfileDto FkUserProfile { get; set; }
        public UserConfigurationDto FkUserConfiguration { get; set; }
        public List<RoleModulePrivilege> Ability { get; set; }
    }

    public class UserLogin
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public DateTime UserDate { get; set; }

    }

    public class UserChangePassword
    {
        [Required]
        public Guid ID { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class UserChangeStatus
    {
        [Required]
        public Guid ID { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }

    public class UserProfileDto
    {
        public Guid Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Gender { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        public string CountryCode { get; set; }
    }
}
