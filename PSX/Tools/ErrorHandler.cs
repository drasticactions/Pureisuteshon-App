using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation.Entities.Web;

namespace PlayStation.Tools
{
    public class ErrorHandler
    {
        public static Result CreateErrorObject(Result result, string reason, string stacktrace, string type = "")
        {
            result.IsSuccess = false;
            var error = new Error()
            {
                Type = type,
                Reason = reason,
                StackTrace = stacktrace
            };
            result.ResultJson = JsonConvert.SerializeObject(error);
            return result;
        }
    }
}
