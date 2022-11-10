using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Shared.SystemEnums;

namespace Shared
{
    public class AppSettings
    {
        public string MaxUsersCount { get; set; }
        public string Secret { get; set; }
        public bool ToSeed { get; set; }
        public string LDAP_DOMAIN { get; set; }
        public string ProductionURL { get; set; }
        public string DevelopmentURL { get; set; }
        public string TimeZone { get; set; }
        public EmailSettings EmailSettings { get; set; }
        public List<EmailTemplate> EmailTemplates { get; set; }
        public string TaskyHost { get; set; }

    }

    public class EmailSettings
    {
        public string SMTPAddress { get; set; }
        public int SMTPPort { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class EmailTemplate
    {
        public List<string> Attachments { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public List<string> Images { get; set; }
    }
}
