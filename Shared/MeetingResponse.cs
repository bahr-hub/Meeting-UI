
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared
{
    public class Response<T>
    {
        public List<T> Meetings { get; set; }
        public MeetingTypeEnum Type { get; set; }

    }
}
