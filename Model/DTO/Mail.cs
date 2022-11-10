using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Model.DTO
{
    public class Mail
    {
        public UserDto From { get; set; }
        public List<UserDto>To { get; set; }
        public string Subject { get; set; }
        public string Html { get; set; }
        public string Text { get; set; }
        public IEnumerable<string> Attachments { get; set; }
    }
}
