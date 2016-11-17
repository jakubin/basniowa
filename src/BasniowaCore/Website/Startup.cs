using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Logic;
using DataAccess;
using Microsoft.IdentityModel.Tokens;
using Website.Infrastructure;
using AutoMapper;
using System.IO;
using System.Reflection;
using Common.Startup;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using System;
using Autofac.Features.ResolveAnything;
using Common.Cqrs;
using Logic.Services;
using Microsoft.Extensions.Caching.Memory;
using DataAccess.UniqueId;

namespace Website
{
    public class Startup
    {
        public static readonly string TheaterDbConnectionStringKey = "TheaterDb";

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public static SymmetricSecurityKey SigningKey;

        public IConfigurationRoot Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDataProtection();
            services.AddOptions();
            services.AddMvc();
            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                var currentAssemblyPath = typeof(Startup).GetTypeInfo().Assembly.Location;
                var xmlCommentsPath = Path.ChangeExtension(currentAssemblyPath, "xml");
                
                options.IncludeXmlComments(xmlCommentsPath);
                options.DescribeAllEnumsAsStrings();

                options.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
            });

            services.AddJwtBearerAuthentication(Configuration);

            services.AddMemoryCache();
            services.AddLogging();
            services.AddSingleton(GetDbOptions);
            services.AddTransient<TheaterDb>();

            var builder = new ContainerBuilder();
            builder.Populate(services);
            
            builder.RegisterType<DbContextFactory>().As<IDbContextFactory>().SingleInstance();
            RegisterUniqueIdProvider(builder);

            builder.RegisterType<ServiceProviderHandlerResolver>().As<IHandlerResolver>().SingleInstance();
            builder.RegisterType<SynchronousMessageBus>()
                .As<ICommandPublisher>()
                .As<IEventPublisher>()
                .SingleInstance();

            builder
                .Register(c => new BufferingUniqueIdService(c.Resolve<IUniqueIdProvider>(), 10))
                .As<IUniqueIdService>()
                .SingleInstance();
            builder.RegisterType<DateTimeService>().As<IDateTimeService>().SingleInstance();
            builder.RegisterType<ShowsProvider>().As<IShowsProvider>().InstancePerDependency();

            builder.RegisterAssemblyTypes(Assembly.Load(new AssemblyName("Logic")))
                .AsClosedTypesOf(typeof(IHandler<>))
                .PropertiesAutowired();

            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            var container = builder.Build();
            return new AutofacServiceProvider(container);
        }

        private DbContextOptions<TheaterDb> GetDbOptions(IServiceProvider serviceProvider)
        {
            var builder = new DbContextOptionsBuilder<TheaterDb>();
            builder.UseMemoryCache(serviceProvider.GetService<IMemoryCache>())
                .UseLoggerFactory(serviceProvider.GetService<ILoggerFactory>());
            ConfigureDbOptions(builder);
            return builder.Options;
        }

        private void ConfigureMapper(IMapperConfigurationExpression cfg)
        {
            Initializer.Init<MapperStartupAttribute>(cfg);
        }

        protected virtual void ConfigureDbOptions(DbContextOptionsBuilder<TheaterDb> options)
        {
            var connectionString = Configuration.GetConnectionString(TheaterDbConnectionStringKey);
            options.UseSqlServer(connectionString);
        }

        protected virtual void RegisterUniqueIdProvider(ContainerBuilder builder)
        {
            var connectionString = Configuration.GetConnectionString(TheaterDbConnectionStringKey);
            builder
                .Register(c => new SqlUniqueIdProvider(connectionString))
                .As<IUniqueIdProvider>()
                .SingleInstance();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.ConfigureJwtBearerAuthentication();
            app.UseMvc();
            app.UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = new[] { "index.htm" } });
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUi();

            Mapper.Initialize(ConfigureMapper);
        }
    }
}
