using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace WebUI.Infrastructure
{
    public class DateModelBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinder binder =
            new DateModelBinder();
        //new SimpleTypeModelBinder(typeof(DateTime));

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return (context.Metadata.ModelType == typeof(DateTime?) || context.Metadata.ModelType == typeof(DateTime)) 
                ? binder : null;
        }
    }
}