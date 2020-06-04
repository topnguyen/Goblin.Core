using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Goblin.Core.Web.Binders
{
    public class GoblinDoubleModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            try
            {
                var value = valueProviderResult.FirstValue?.Trim();

                var model = string.IsNullOrWhiteSpace(value)
                    ? (double?) null
                    : Convert.ToDouble(value, CultureInfo.InvariantCulture);

                // If model is null and type is not nullable Return a required field error

                if (model == null && !bindingContext.ModelMetadata.IsReferenceOrNullableType)
                {
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName,
                        bindingContext.ModelMetadata.ModelBindingMessageProvider.ValueMustNotBeNullAccessor(
                            valueProviderResult.ToString()));

                    return Task.CompletedTask;
                }

                bindingContext.Result = ModelBindingResult.Success(model);

                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                bindingContext.ModelState
                    .TryAddModelError(bindingContext.ModelName, exception, bindingContext.ModelMetadata);

                return Task.CompletedTask;
            }
        }
    }

    public class DoubleModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Metadata.UnderlyingOrModelType == typeof(double) ? new GoblinDoubleModelBinder() : null;
        }
    }

    public static class DoubleModelBinderExtensions
    {
        public static IServiceCollection AddDoubleBinder(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(options =>
            {
                var isProviderAdded =
                    options.ModelBinderProviders.Any(x => x.GetType() == typeof(DoubleModelBinderProvider));

                if (isProviderAdded) return;

                options.ModelBinderProviders.Insert(0, new DoubleModelBinderProvider());
            });

            return services;
        }
    }
}