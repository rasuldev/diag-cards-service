using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebUI.Infrastructure
{
    public class DateModelBinder : IModelBinder
    {
        private readonly IModelBinder fallbackBinder;
        public DateModelBinder(IModelBinder fallbackBinder)
        {
            this.fallbackBinder = fallbackBinder;
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var dateStr = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (dateStr.Length == 0) return Task.CompletedTask;

            if (DateTime.TryParseExact(dateStr.FirstValue, "dd.MM.yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime date))
            {
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, date, dateStr.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(date);
                return Task.CompletedTask;
            }
            return fallbackBinder.BindModelAsync(bindingContext);
        }
    }
}