using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class Result
    {
        public bool Success { get; set; }
        public dynamic Data { get; set; }
        public string Message { get; set; }
        public long Count { get; set; }

        public Result(bool Success)
        {
            this.Success = Success;
        }
    }
    public class Success : Result
    {
        public Success(DataSourceResult DataSourceResult, string Message = "Success") : base(true)
        {
            if (DataSourceResult != null && DataSourceResult.Data != null)
            {
                this.Data = DataSourceResult.Data;
                this.Count = DataSourceResult.Count;
                this.Message = Message;
            }
        }

        public Success(dynamic Data, string Message = "Success") : base(true)
        {
            this.Data = Data;
            this.Message = Message;
        }

    }
    public class Error : Result
    {
        public string Message { get; set; }
        public Error(string Message) : base(false)
        {
            this.Message = Message;

        }
    }
}
