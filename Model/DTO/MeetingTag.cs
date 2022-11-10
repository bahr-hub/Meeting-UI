using System;
using System.Collections.Generic;

namespace Model.DTO
{
    public class MeetingTagDto
    {
        public Guid Id { get; set; }
        public Guid MeetingId { get; set; }
        public int TagId { get; set; }

     
        public TagDto Tag { get; set; }
    }
}
