using System;
using System.Collections.Generic;

namespace Model.Models
{
    public partial class User
    {
        public User()
        {
            InverseCreatedByNavigation = new HashSet<User>();
            InverseUpdatedByNavigation = new HashSet<User>();
            LocationCreatedByNavigation = new HashSet<Location>();
            LocationUpdatedByNavigation = new HashSet<Location>();
            MeetingCreatedByNavigation = new HashSet<Meeting>();
            MeetingParticipant = new HashSet<MeetingParticipant>();
            MeetingTaskAssignee = new HashSet<MeetingTask>();
            MeetingTaskCreatedByNavigation = new HashSet<MeetingTask>();
            MeetingTaskUpdatedByNavigation = new HashSet<MeetingTask>();
            MeetingTopicCreatedByNavigation = new HashSet<MeetingTopic>();
            MeetingTopicPresenter = new HashSet<MeetingTopic>();
            MeetingTopicUpdatedByNavigation = new HashSet<MeetingTopic>();
            MeetingUpdatedByNavigation = new HashSet<Meeting>();
            Notification = new HashSet<Notification>();
            ProjectCreatedByNavigation = new HashSet<Project>();
            ProjectUpdatedByNavigation = new HashSet<Project>();
            Proposals = new HashSet<Proposals>();
            RoleCreatedByNavigation = new HashSet<Role>();
            RoleUpdatedByNavigation = new HashSet<Role>();
            TagCreatedByNavigation = new HashSet<Tag>();
            TagUpdatedByNavigation = new HashSet<Tag>();
            UserProfileCreatedByNavigation = new HashSet<UserProfile>();
            UserProfileUpdatedByNavigation = new HashSet<UserProfile>();
            UserRole = new HashSet<UserRole>();
            VacationCreatedByNavigation = new HashSet<Vacation>();
            VacationFkUser = new HashSet<Vacation>();
            VacationUpdatedByNavigation = new HashSet<Vacation>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public Guid FkUserProfileId { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? FkUserConfigurationId { get; set; }
        public string ImageUrl { get; set; }
        public int? LocationID { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual UserConfiguration FkUserConfiguration { get; set; }
        public virtual UserProfile FkUserProfile { get; set; }
        public virtual User UpdatedByNavigation { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<User> InverseCreatedByNavigation { get; set; }
        public virtual ICollection<User> InverseUpdatedByNavigation { get; set; }
        public virtual ICollection<Location> LocationCreatedByNavigation { get; set; }
        public virtual ICollection<Location> LocationUpdatedByNavigation { get; set; }
        public virtual ICollection<Meeting> MeetingCreatedByNavigation { get; set; }
        public virtual ICollection<MeetingParticipant> MeetingParticipant { get; set; }
        public virtual ICollection<MeetingTask> MeetingTaskAssignee { get; set; }
        public virtual ICollection<MeetingTask> MeetingTaskCreatedByNavigation { get; set; }
        public virtual ICollection<MeetingTask> MeetingTaskUpdatedByNavigation { get; set; }
        public virtual ICollection<MeetingTopic> MeetingTopicCreatedByNavigation { get; set; }
        public virtual ICollection<MeetingTopic> MeetingTopicPresenter { get; set; }
        public virtual ICollection<MeetingTopic> MeetingTopicUpdatedByNavigation { get; set; }
        public virtual ICollection<Meeting> MeetingUpdatedByNavigation { get; set; }
        public virtual ICollection<Notification> Notification { get; set; }
        public virtual ICollection<Project> ProjectCreatedByNavigation { get; set; }
        public virtual ICollection<Project> ProjectUpdatedByNavigation { get; set; }
        public virtual ICollection<Proposals> Proposals { get; set; }
        public virtual ICollection<Role> RoleCreatedByNavigation { get; set; }
        public virtual ICollection<Role> RoleUpdatedByNavigation { get; set; }
        public virtual ICollection<Tag> TagCreatedByNavigation { get; set; }
        public virtual ICollection<Tag> TagUpdatedByNavigation { get; set; }
        public virtual ICollection<UserProfile> UserProfileCreatedByNavigation { get; set; }
        public virtual ICollection<UserProfile> UserProfileUpdatedByNavigation { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
        public virtual ICollection<Vacation> VacationCreatedByNavigation { get; set; }
        public virtual ICollection<Vacation> VacationFkUser { get; set; }
        public virtual ICollection<Vacation> VacationUpdatedByNavigation { get; set; }
    }
}
