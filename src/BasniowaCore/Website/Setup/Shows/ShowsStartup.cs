using System.IO;
using Autofac;
using Common.FileContainers;
using DataAccess.Disk;
using Logic.Shows;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Website.Api.Shows;
using Website.Infrastructure.FileContainers;

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

            builder.Register(ctx => new PhysicalFileContainer(
                ResolveShowPictureContainerRootPath(
                    ctx.Resolve<IHostingEnvironment>(),
                    ctx.Resolve<IOptions<ShowsOptions>>())))
                .Named<IFileContainer>(ShowPicturesContainerKey)
                .Named<IFileContainerReader>(ShowPicturesContainerKey)
                .SingleInstance();

            builder.Register(ctx => new PhysicalStaticFileContainerUrlProvider("/files/shows/pictures"))
                .Named<IFileContainerUrlProvider>(ShowPicturesContainerKey)
                .SingleInstance();

            builder.Register(
                ctx => new ShowPicturesFileContainer(ctx.ResolveNamed<IFileContainer>(ShowPicturesContainerKey)));
            builder.Register(
                ctx =>
                    new ShowPicturesFileContainerReader(ctx.ResolveNamed<IFileContainerReader>(ShowPicturesContainerKey)));
            builder.Register(
                ctx => new ShowPictureUrlProvider(ctx.ResolveNamed<IFileContainerUrlProvider>(ShowPicturesContainerKey)));
        }

        /// <summary>
        /// Configures the shows module in the application.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void UseShowsModule(this IApplicationBuilder app)
        {
            var showsPicturesRootPath = ResolveShowPictureContainerRootPath(
                app.ApplicationServices.GetRequiredService<IHostingEnvironment>(),
                app.ApplicationServices.GetRequiredService<IOptions<ShowsOptions>>());
            
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(showsPicturesRootPath),
                RequestPath = new PathString("/files/shows/pictures")
            });
        }

        private static string ResolveShowPictureContainerRootPath(
            IHostingEnvironment hostingEnvironment, IOptions<ShowsOptions> options)
        {
            var path = Path.IsPathRooted(options.Value.ShowPictureContainerRoot)
                ? options.Value.ShowPictureContainerRoot
                : Path.Combine(hostingEnvironment.ContentRootPath, options.Value.ShowPictureContainerRoot);

            return path;
        }
    }
}