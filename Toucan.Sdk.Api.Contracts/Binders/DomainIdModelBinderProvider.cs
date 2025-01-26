using Microsoft.AspNetCore.Mvc.ModelBinding;
using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Api.Binders;
public sealed class DomainIdModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
            return Task.CompletedTask;

        string? value = valueProviderResult.FirstValue;
        if (string.IsNullOrWhiteSpace(value))
            bindingContext.Result = ModelBindingResult.Success(DomainId.Empty);
        else
        {
            try
            {
                bindingContext.Result = ModelBindingResult.Success(DomainId.Parse(value));
            }
            catch (Exception)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid DomainId format");
            }
        }

        return Task.CompletedTask;
    }
}
internal class DomainIdModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(DomainId))
            return new DomainIdModelBinder();

        return null;
    }
}
