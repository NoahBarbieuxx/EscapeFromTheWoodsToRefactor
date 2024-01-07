using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escape.BL.Models
{
    public class Log
    {
        public Log(int logId, int woodId, int monkeyId, string message)
        {
            LogId = logId;
            WoodId = woodId;
            MonkeyId = monkeyId;
            Message = message;
        }

        public int LogId { get; set; }
        public int WoodId { get; set; }
        public int MonkeyId { get; set; }
        public string Message { get; set; }
    }
}