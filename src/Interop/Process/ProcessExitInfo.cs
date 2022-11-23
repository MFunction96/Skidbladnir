using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skidbladnir.Interop.Process
{
    public class ProcessExitInfo
    {
        public int ExitCode { get; set; }
        public string Output { get; set; }
        public string Error { get; set; }

        public void Append(ProcessExitInfo info)
        {
            this.ExitCode += info.ExitCode;
            this.Output += info.Output;
            this.Error += info.Error;
        }
    }
}
