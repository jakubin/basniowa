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
        public static IApplicationBuilder UseCqrsLogging(this IApplicationBuilder app)
        {
            app.UseCqrsCommandHandlingLogging();
            app.UseCqrsEventHandlingLogging();
            app.UseCqrsEventPublishingLogging();

            return app;
        }

        /// <summary>
        /// Uses the CQRS command logging processor.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>The application (same as <paramref name="app"/>).</returns>
        public static IApplicationBuilder UseCqrsCommandHandlingLogging(this IApplicationBuilder app)
        {
            var bus = app.ApplicationServices.GetRequiredService<ICommandBus>();
            var processor = app.ApplicationServices.GetRequiredService<CommandHandlingLogger>();

            bus.AddCommandHandlerProcessor(processor);

            return app;
        }

        /// <summary>
        /// Uses the CQRS event handling logging processor.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>The application (same as <paramref name="app"/>).</returns>
        public static IApplicationBuilder UseCqrsEventHandlingLogging(this IApplicationBuilder app)
        {
            var bus = app.ApplicationServices.GetRequiredService<IEventBus>();
            var processor = app.ApplicationServices.GetRequiredService<EventHandlingLogger>();

            bus.AddEventHandlerProcessor(processor);

            return app;
        }

        /// <summary>
        /// Uses the CQRS event publishing logging processor.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>The application (same as <paramref name="app"/>).</returns>
        public static IApplicationBuilder UseCqrsEventPublishingLogging(this IApplicationBuilder app)
        {
            var bus = app.ApplicationServices.GetRequiredService<IEventBus>();
            var processor = app.ApplicationServices.GetRequiredService<EventPublishingLogger>();

            bus.AddEventPublicationProcessor(processor);

            return app;
        }
    }
}
