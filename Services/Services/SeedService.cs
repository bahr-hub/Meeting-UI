using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model.DTO;
using Model.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Services
{
    public static class SeedService
    {
        static List<Module> modules = new List<Module>()
        {
            new Module()
            {
                Name = "task",
                Description = "task",
                IsActive = true
            },
            new Module()
            {
                Name = "meeting",
                Description = "meeting",
                IsActive = true
            },
            new Module()
            {
                Name = "user",
                Description = "user",
                IsActive = true
            },
            new Module()
            {
                Name = "location",
                Description = "location",
                IsActive = true
            },
            new Module()
            {
                Name = "project",
                Description = "project",
                IsActive = true
            },
            new Module()
            {
                Name = "tag",
                Description = "tag",
                IsActive = true
            },
            new Module()
            {
                Name = "role",
                Description = "role",
                IsActive = true
            },
            new Module()
            {
                Name = "systemconfiguration",
                Description = "systemconfiguration",
                IsActive = true
            },
            new Module()
            {
                Name = "vacation",
                Description = "vacation",
                IsActive = true
            }

        };

        static List<Privilege> privileges = new List<Privilege>()
        {
            new Privilege()
            {
                Name = "view",
                Description = "view",
                IsActive = true
            },
            new Privilege()
            {
                Name = "create",
                Description = "create",
                IsActive = true
            },
            new Privilege()
            {
                Name = "edit",
                Description = "edit",
                IsActive = true
            },
            new Privilege()
            {
                Name = "details",
                Description = "details",
                IsActive = true
            },
            new Privilege()
            {
                Name = "delete",
                Description = "delete",
                IsActive = true
            },
            new Privilege()
            {
                Name = "delay",
                Description = "delay",
                IsActive = true
            }
        };

        static List<SystemConfiguration> systemConfiguration = new List<SystemConfiguration>()
        {
            new SystemConfiguration()
            {
                Id=1,
                IntegrationWithGoogleCalendar=false,
                AuthenticationMode=0,
                MaxMeetingTime=5,
                StartOfWorkDays= (int)SystemEnums.WeekDays.Sun,
                EndOfWorkDays= (int)SystemEnums.WeekDays.Thu,
                TimeZone = "UTC"

            }
        };
        public static bool Seed(BaseDataBaseContext Context)
        {
            try
            {
                Context.AddRange(privileges.Where(x => !Context.Privilege.Any(y => y.Name == x.Name)));
                Context.SaveChanges();
                Context.AddRange(modules.Where(x => !Context.Module.Any(y => y.Name == x.Name)));
                Context.SaveChanges();
                Context.AddRange(systemConfiguration.Where(x => !Context.SystemConfiguration.Any(y => y.Id == x.Id)));
                Context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                var log = e;
                return false;
            }
        }

    }
}
