namespace Xanadu.Skidbladnir.Interop.Process
{
    public class ProcessExitInfo
    {
        public int ExitCode { get; set; }
        public string Output { get; set; }
        public string Error { get; set; }

        public void Append(ProcessExitInfo info)
        {
            this.ExitCode = info.ExitCode;
            this.Output += info.Output;
            this.Error += info.Error;
        }

        public override string ToString()
        {
            return
                $"{nameof(this.ExitCode)}: {this.ExitCode}\r\n{nameof(this.Output)}: \r\n{this.Output}\r\n{nameof(this.Error)}: \r\n{this.Error}";
        }
    }
}
