using Common.Cqrs;
using Logic.Common.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Website.Infrastructure
{
    /// <summary>
    /// <see cref="IApplicationBuilder"/> extensions adding CQRS logging.
    /// </summary>
    public static class CqrsLoggingApplicationBuilderExtensions
    {
        /// <summary>
        /// Uses the CQRS command logging processor.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>The application (same as <paramref name="app"/>).</returns>
        public static IApplicationBuilder UseCqrsCommandLogging(this IApplicationBuilder app)
        {
            var commandBus = app.ApplicationServices.GetRequiredService<ICommandBus>();
            var processor = app.ApplicationServices.GetRequiredService<CommandHandlingLogger>();

            commandBus.AddCommandHandlerProcessor(processor);

            return app;
        }
    }
}
