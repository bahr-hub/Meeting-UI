using AutoMapper;
using Model;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<User, Model.DTO.UserDto>().ReverseMap();
            CreateMap<Role, Model.DTO.RoleDto>().ReverseMap();
            CreateMap<Tag, Model.DTO.TagDto>().ReverseMap();
            CreateMap<Project, Model.DTO.ProjectDto>().ReverseMap();
            CreateMap<Location, Model.DTO.LocationDto>().ReverseMap();
            CreateMap<Role, Model.DTO.RoleDto>().ReverseMap();
            CreateMap<RoleModulePrivilege, Model.DTO.RoleModulePrivilegeDto>().ReverseMap();
            CreateMap<UserRole, Model.DTO.UserRoleDto>().ReverseMap();
            CreateMap<UserProfile, Model.DTO.UserProfileDto>().ReverseMap();
            CreateMap<Meeting, Model.DTO.MeetingDto>().ReverseMap();
            CreateMap<MeetingParticipant, Model.DTO.MeetingParticipantDto>().ReverseMap();
            CreateMap<MeetingTask, Model.DTO.MeetingTaskDto>().ReverseMap();
            CreateMap<MeetingTopic, Model.DTO.MeetingTopicDto>().ReverseMap();
            CreateMap<MeetingTag, Model.DTO.MeetingTagDto>().ReverseMap();
            CreateMap<UserConfiguration, Model.DTO.UserConfigurationDto>().ReverseMap();
            CreateMap<Vacation, Model.DTO.VacationDto>().ReverseMap();
            CreateMap<Notification, Model.DTO.NotificationDto>().ReverseMap();
            CreateMap<Module, Model.DTO.ModuleDto>().ReverseMap();
            CreateMap<Privilege, Model.DTO.PrivilegeDto>().ReverseMap();
        }
    }
}
