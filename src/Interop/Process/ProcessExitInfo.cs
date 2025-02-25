namespace Xanadu.Skidbladnir.Interop.Process
{
    /// <summary>
    /// Process exit information.
    /// </summary>
    public class ProcessExitInfo
    {
        /// <summary>
        /// Exit code.
        /// </summary>
        public int ExitCode { get; set; }

        /// <summary>
        /// Standard output.
        /// </summary>
        public string Output { get; set; } = string.Empty;

        /// <summary>
        /// Standard error.
        /// </summary>
        public string Error { get; set; } = string.Empty;

        /// <summary>
        /// Append exit information.
        /// </summary>
        /// <param name="info">ProcessExitInfo to append.</param>
        public void Append(ProcessExitInfo info)
        {
            this.ExitCode = info.ExitCode;
            this.Output += info.Output;
            this.Error += info.Error;
        }

        /// <summary>
        /// Convert to string.
        /// </summary>
        /// <returns>String of instance. Not JSON String.</returns>
        public override string ToString()
        {
            return
                $"{nameof(this.ExitCode)}: {this.ExitCode}\r\n{nameof(this.Output)}: \r\n{this.Output}\r\n{nameof(this.Error)}: \r\n{this.Error}";
        }
    }
}
