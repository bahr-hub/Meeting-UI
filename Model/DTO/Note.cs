using System;
using System.ComponentModel.DataAnnotations;

namespace Model.DTO
{
    public partial class NoteDto
    {
        public Guid MeetingID { get; set; }
        public string Description { get; set; }
    }
}
