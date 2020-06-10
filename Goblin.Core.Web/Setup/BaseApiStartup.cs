using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text.Json;
using Elect.Core.ConfigUtils;
using Elect.DI;
using Elect.Job.Hangfire;
using Elect.Job.Hangfire.Models;
using Elect.Logger.Logging;
using Elect.Logger.Logging.Models;
using Elect.Mapper.AutoMapper;
using Elect.Web.HealthCheck;
using Elect.Web.HealthCheck.Models;
using Elect.Web.Middlewares.CorsMiddleware;
using Elect.Web.Middlewares.HttpContextMiddleware;
using Elect.Web.Middlewares.MeasureProcessingTimeMiddleware;
using Elect.Web.Middlewares.ServerInfoMiddleware;
using Elect.Web.Swagger;
using Elect.Web.Swagger.Models;
using FluentValidation.AspNetCore;
using Goblin.Core.Web.Binders;
using Goblin.Core.Web.Filters.Exception;
using Goblin.Core.Web.Filters.Validation;
using Goblin.Core.Web.JsonConverters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Goblin.Core.Web.Setup
{
    public abstract class BaseApiStartup
    {
        protected readonly IWebHostEnvironment WebHostEnvironment;

        protected readonly IConfiguration Configuration;

        protected Action<IServiceCollection> BeforeConfigureServices { get; set; }

        protected Action<IServiceCollection> AfterConfigureServices { get; set; }

        protected Action<IMvcCoreBuilder> BeforeConfigureMvc { get; set; }

        protected Action<IMvcCoreBuilder> AfterConfigureMvc { get; set; }

        protected Action<IApplicationBuilder, IWebHostEnvironment, IHostApplicationLifetime> BeforeConfigureApp
        {
            get;
            set;
        }

        protected Action<IApplicationBuilder, IWebHostEnvironment, IHostApplicationLifetime> AfterConfigureApp
        {
            get;
            set;
        }

        /// <summary>
        ///     Config for Fluent Validation - Register Validators From Assembly Containing Types
        /// </summary>
        protected List<Type> RegisterValidators { get; set; } = new List<Type>();

        protected BaseApiStartup(IWebHostEnvironment env, IConfiguration configuration)
        {
            WebHostEnvironment = env;

            Configuration = configuration;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            // Call Back
            BeforeConfigureServices?.Invoke(services);

            // Logger
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            var electLogOptions = Configuration.GetSection<ElectLogOptions>("ElectLog");
            services.AddElectLog(electLogOptions);

            // Mapper
            services.AddElectAutoMapper();

            // Http Context
            services.AddElectHttpContext();

            // Server Info
            services.AddElectServerInfo();

            // API Doc - Swagger
            var electSwaggerOptions = Configuration.GetSection<ElectSwaggerOptions>("ElectSwagger");
            services.AddElectSwagger(electSwaggerOptions);

            // Health Check
            var electHealthCheckOptions = Configuration.GetSection<ElectHealthCheckOptions>("ElectHealthCheck");
            services.AddElectHealthCheck(electHealthCheckOptions);

            // Background Job - Hangfire
            var electHangfireOptions = Configuration.GetSection<ElectHangfireOptions>("ElectHangfire");
            services.AddElectHangfire(electHangfireOptions);

            // MVC

            // MVC Core
            var mvcCoreBuilder = services.AddMvcCore(options => { options.EnableEndpointRouting = false; });

            // Callback
            BeforeConfigureMvc?.Invoke(mvcCoreBuilder);

            mvcCoreBuilder.AddDataAnnotations();

            mvcCoreBuilder.AddApiExplorer(); // API Doc - Swagger Needed

            mvcCoreBuilder.AddFormatterMappings();
            
            // Action Context Accessor
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            // Model Binder
            services.AddDateTimeOffsetBinder();
            services.AddDateTimeBinder();
            services.AddTimeSpanBinder();

            // Filters
            services.AddScoped<GoblinApiValidationActionFilterAttribute>();
            services.AddScoped<GoblinApiExceptionFilterAttribute>();

            // MVC Json Config
            mvcCoreBuilder.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new DateTimeOffsetJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new TimeSpanJsonConverter());
            });

            // Fluent Validation
            mvcCoreBuilder.AddFluentValidation(fvc =>
            {
                foreach (var type in RegisterValidators)
                {
                    fvc.RegisterValidatorsFromAssemblyContaining(type);
                }

                fvc.ImplicitlyValidateChildProperties = true;
            });

            // Callback
            AfterConfigureMvc?.Invoke(mvcCoreBuilder);

            // Cors
            services.AddElectCors();

            // Response Compress

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            // DI
            services.AddElectDI();
            services.PrintServiceAddedToConsole();
            Console.ResetColor();

            // Call Back
            AfterConfigureServices?.Invoke(services);
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IHostApplicationLifetime lifetime)
        {
            // Call Back
            BeforeConfigureApp?.Invoke(app, env, lifetime);

            // Log
            app.UseElectLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Cors
            app.UseElectCors();

            // Http Context
            app.UseElectHttpContext();

            // Service Measure
            app.UseElectMeasureProcessingTime();

            // Server Info
            app.UseElectServerInfo();

            // Response Compress
            app.UseResponseCompression();

            // API Doc - Swagger
            app.UseElectSwagger();

            // Health Check
            app.UseElectHealthCheck();

            // Background Job - Hangfire
            app.UseElectHangfire();

            // Static Files
            app.UseStaticFiles();

            // MVC
            app.UseMvcWithDefaultRoute();

            // Call Back
            AfterConfigureApp?.Invoke(app, env, lifetime);
        }
    }
}