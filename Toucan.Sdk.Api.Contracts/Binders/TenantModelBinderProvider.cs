using Microsoft.AspNetCore.Mvc.ModelBinding;
using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Api.Binders;

internal class TenantModelBinderProvider : IModelBinderProvider
{
    private class TenantModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);

            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            string? value = valueProviderResult.FirstValue;
            if (string.IsNullOrWhiteSpace(value))
                bindingContext.Result = ModelBindingResult.Success(Tenant.Unspecified);
            else
            {
                try
                {
                    bindingContext.Result = ModelBindingResult.Success(Tenant.Parse(value));
                }
                catch (Exception)
                {
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid Tenant format");
                }
            }

            return Task.CompletedTask;
        }
    }

    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(Tenant))
            return new TenantModelBinder();

        return null;
    }
}
