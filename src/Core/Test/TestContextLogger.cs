using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Xanadu.Skidbladnir.Core.Test
{
    /// <summary>
    /// MSTest's TestContext based logger. It writes log messages to the test output, which can be viewed in the test results.
    /// </summary>
    /// <typeparam name="T">The type whose name is used for the logger category.</typeparam>
    /// <param name="testContext">The MSTest TestContext instance.</param>
    public class TestContextLogger<T>(TestContext testContext) : ILogger<T>
    {
        /// <inheritdoc/>
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        /// <inheritdoc/>
        public bool IsEnabled(LogLevel logLevel) => true;

        /// <inheritdoc/>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);
            testContext.WriteLine($"[{logLevel}] {message}");

            if (exception is not null)
            {
                testContext.WriteLine(exception.ToString());
            }
        }
    }
}
