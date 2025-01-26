using Microsoft.AspNetCore.Mvc.ModelBinding;
using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Api.Binders;

internal class TagModelBinderProvider : IModelBinderProvider
{
    private class TagModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);

            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            string? value = valueProviderResult.FirstValue;
            if (string.IsNullOrWhiteSpace(value))
                bindingContext.Result = ModelBindingResult.Success(Tag.Empty);
            else
            {
                try
                {
                    bindingContext.Result = ModelBindingResult.Success(Tag.Parse(value));
                }
                catch (Exception)
                {
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid Tag format");
                }
            }

            return Task.CompletedTask;
        }
    }

    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(Tag))
            return new TagModelBinder();

        return null;
    }
}
