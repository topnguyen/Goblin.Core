using System;
using System.IO.Compression;
using Elect.DI;
using Elect.Mapper.AutoMapper;
using Elect.Web.Middlewares.HttpContextMiddleware;
using Elect.Web.Middlewares.MeasureProcessingTimeMiddleware;
using Elect.Web.Middlewares.ServerInfoMiddleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace Goblin.Core.Web.Setup
{
    public abstract class BaseBlazorStartup
    {
        protected readonly IWebHostEnvironment WebHostEnvironment;

        protected readonly IConfiguration Configuration;

        protected Action<IServiceCollection> BeforeConfigureServices { get; set; }

        protected Action<IServiceCollection> AfterConfigureServices { get; set; }

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
        
        protected BaseBlazorStartup(IWebHostEnvironment env, IConfiguration configuration)
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

            // Mapper
            services.AddElectAutoMapper();

            // Http Context
            services.AddElectHttpContext();

            // Server Info
            services.AddElectServerInfo();

            // Blazor
            services.AddHeadElementHelper();
            
            services.AddRazorPages();
            services.AddServerSideBlazor();

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

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            // Call Back
            BeforeConfigureApp?.Invoke(app, env, lifetime);

            // Blazor Metadata Render Middleware (Keep on the top / first Middleware)
            app.UseHeadElementServerPrerendering();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Http Context
            app.UseElectHttpContext();

            // Service Measure
            app.UseElectMeasureProcessingTime();

            // Server Info
            app.UseElectServerInfo();

            // Response Compress
            app.UseResponseCompression();

            // Static Files

            app.UseStaticFiles();
            
            // Blazor
            
            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
            
            // Call Back
            AfterConfigureApp?.Invoke(app, env, lifetime);
        }
    }
}