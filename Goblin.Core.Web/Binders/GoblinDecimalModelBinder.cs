using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Goblin.Core.Web.Binders
{
    public class GoblinDecimalModelBinder : IModelBinder
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
                    ? (decimal?) null
                    : Convert.ToDecimal(value, CultureInfo.InvariantCulture);

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
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, exception,
                    bindingContext.ModelMetadata);

                return Task.CompletedTask;
            }
        }
    }

    public class DecimalModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Metadata.UnderlyingOrModelType == typeof(decimal) ? new GoblinDecimalModelBinder() : null;
        }
    }

    public static class DecimalModelBinderExtensions
    {
        public static IServiceCollection AddDecimalBinder(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(options =>
            {
                var isProviderAdded =
                    options.ModelBinderProviders.Any(x => x.GetType() == typeof(DecimalModelBinderProvider));

                if (isProviderAdded)
                {
                    return;
                }

                options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
            });

            return services;
        }
    }
}