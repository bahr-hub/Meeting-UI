using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Model.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{    public interface ILiecenseService : IBaseService
    {
        
        bool IsValidLiecense();
        string Encrypt(string value);

    }
    public class LiecenseService : BaseService, ILiecenseService
    {
        public LiecenseService(IOptions<AppSettings> appSettings, IMapper mapper, BaseDataBaseContext context, IHttpContextAccessor contextAccessor, IHostingEnvironment env) : base(appSettings, mapper, context, contextAccessor, env)
        {
        }

        public string Encrypt(string value)
        {
            return Shared.Security.TripleDES.Encrypt(value, true);
        }

        private string Decrypt(string value)
        {
            return Shared.Security.TripleDES.Decrypt(value, true);
        }

        public bool IsValidLiecense()
        {
            int maxUsersCount = 0;
            try
            {
                // "/Aywn4tXolM ="
                
                if (int.TryParse(Decrypt(_appSettings.MaxUsersCount), out maxUsersCount))
                {

                    int usersCount = _Context.User.Count(x => x.IsDeleted == false);
                    if (usersCount <= maxUsersCount)
                        return true;
                }
            }
            catch (Exception e) {
                return false;
            }
            return false;
        }
    }
}
