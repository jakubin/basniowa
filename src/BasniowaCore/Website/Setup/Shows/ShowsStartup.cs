using System.IO;
using Autofac;
using Common.FileContainers;
using DataAccess.Disk;
using Logic.Shows;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Website.Setup.Shows
{
    /// <summary>
    /// Startup helper methods for Shows module.
    /// </summary>
    public static class ShowsStartup
    {
        private static readonly string ShowPicturesContainerKey = "Shows.ShowPictures";

        /// <summary>
        /// Configures the services collection for shows model.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="showsSection">The shows configuration section.</param>
        public static void ConfigureShows(this IServiceCollection services, IConfigurationSection showsSection)
        {
            services.Configure<ShowsOptions>(showsSection);
        }

        /// <summary>
        /// Configures the container for shows module.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        public static void ConfigureShows(this ContainerBuilder builder)
        {
            builder.RegisterType<ShowsReader>().As<IShowsReader>()
                .InstancePerDependency()
                .PropertiesAutowired();

            builder.Register(ctx => new PhysicalFileContainer(ResolveShowPictureContainerRootPath(ctx)))
                .Named<IFileContainer>(ShowPicturesContainerKey)
                .Named<IFileContainerReader>(ShowPicturesContainerKey)
                .SingleInstance();

            builder.Register(
                ctx => new ShowPicturesFileContainer(ctx.ResolveNamed<IFileContainer>(ShowPicturesContainerKey)));
        }

        private static string ResolveShowPictureContainerRootPath(
            IComponentContext ctx)
        {
            var hostingEnvironment = ctx.Resolve<IHostingEnvironment>();
            var options = ctx.Resolve<IOptions<ShowsOptions>>();
            var path = Path.IsPathRooted(options.Value.ShowPictureContainerRoot)
                ? options.Value.ShowPictureContainerRoot
                : Path.Combine(hostingEnvironment.ContentRootPath, options.Value.ShowPictureContainerRoot);

            return path;
        }
    }
}