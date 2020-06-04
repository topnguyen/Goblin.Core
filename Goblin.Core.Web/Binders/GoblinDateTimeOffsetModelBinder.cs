using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Core.DateTimeUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Goblin.Core.Web.Binders
{
    public class GoblinDateTimeOffsetModelBinder : IModelBinder
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
                var value = valueProviderResult.FirstValue;

                object model = string.IsNullOrWhiteSpace(value) ? null : value.ToSystemDateTime();

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

    public class DateTimeOffsetModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Metadata.UnderlyingOrModelType == typeof(DateTimeOffset)
                ? new GoblinDateTimeOffsetModelBinder()
                : null;
        }
    }

    public static class DateTimeOffsetModelBinderExtensions
    {
        public static IServiceCollection AddDateTimeOffsetBinder(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(options =>
            {
                var isProviderAdded =
                    options.ModelBinderProviders.Any(x => x.GetType() == typeof(DateTimeOffsetModelBinderProvider));

                if (isProviderAdded) return;

                options.ModelBinderProviders.Insert(0, new DateTimeOffsetModelBinderProvider());
            });

            return services;
        }
    }
}