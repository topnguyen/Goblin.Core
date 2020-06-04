using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Goblin.Core.Web.Setup
{
    /// <summary>
    ///     Base Startup for UI project, inherit from Startup for API project with Views Features
    /// </summary>
    public abstract class BaseUiStartup : BaseApiStartup
    {
        protected BaseUiStartup(IWebHostEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
            AfterConfigureMvc = mvcCoreBuilder =>
            {
                // MVC View
                mvcCoreBuilder.AddViews(options => { options.HtmlHelperOptions.ClientValidationEnabled = true; });

                mvcCoreBuilder.AddRazorViewEngine();

                mvcCoreBuilder.AddCacheTagHelper();
            };
        }
    }
}