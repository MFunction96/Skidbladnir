using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xanadu.Skidbladnir.Core.Test
{
    /// <summary>
    /// Extension methods for setting up test logging services. This allows tests to easily configure logging to use the TestContextLogger, which writes log messages to the MSTest test output.
    /// </summary>
    public static class TestExtension
    {
        /// <summary>
        /// Adds logging services to the service collection, configuring the ILogger&lt;T&gt; to use the TestContextLogger&lt;T&gt; implementation. This allows tests to capture log output in the test context, making it easier to diagnose issues during testing.
        /// </summary>
        /// <param name="services">The service collection to add the logging services to.</param>
        /// <param name="testContext">The MSTest TestContext to use for logging.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddTestLogging(this IServiceCollection services, TestContext testContext)
        {
            services.AddLogging();
            services.AddSingleton(testContext);
            services.AddSingleton(typeof(ILogger<>), typeof(TestContextLogger<>));
            return services;
        }
    }
}
