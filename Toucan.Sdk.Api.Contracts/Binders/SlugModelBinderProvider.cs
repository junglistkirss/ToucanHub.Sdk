using Microsoft.AspNetCore.Mvc.ModelBinding;
using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Api.Binders;
public sealed class SlugModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
            return Task.CompletedTask;

        string? value = valueProviderResult.FirstValue;
        if (string.IsNullOrWhiteSpace(value))
            bindingContext.Result = ModelBindingResult.Success(Slug.Empty);
        else
        {
            try
            {
                bindingContext.Result = ModelBindingResult.Success(Slug.Parse(value));
            }
            catch (Exception)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid Slug format");
            }
        }

        return Task.CompletedTask;
    }
}

internal class SlugModelBinderProvider : IModelBinderProvider
{


    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(Slug))
            return new SlugModelBinder();

        return null;
    }
}
