using System.Threading.Tasks;
using EaisApi.Exceptions;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using WebUI.Data.Entities;

namespace WebUI.Infrastructure
{
    public class ExceptionFilter: IAsyncExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.Exception is NotAuthorizedException)
            {
                context.ExceptionHandled = true;
                var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<User>>();
                await signInManager.SignOutAsync();
                var factory = context.HttpContext.RequestServices.GetService(typeof(ITempDataDictionaryFactory)) as ITempDataDictionaryFactory;
                var tempData = factory?.GetTempData(context.HttpContext);
                tempData?.AddErrorMessage("На ЕАИСТО истекла ваша сессия. Войдите еще раз для возобновления сессии.");
                context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl = context.HttpContext.Request.GetUri().PathAndQuery });
            }
        }
    }
}