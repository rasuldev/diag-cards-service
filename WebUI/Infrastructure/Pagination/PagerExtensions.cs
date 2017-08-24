using System.Linq;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WebUI.Infrastructure.Pagination
{
    public static class PagerExtensions
    {
        public static IQueryable<T> GetPage<T>(this IQueryable<T> source,  int pageIndex, int pageSize)
        {
            return source.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        public static void AddPager(this IServiceCollection services)
        {
            //services.TryAddScoped(typeof(IActionContextAccessor), typeof(ActionContextAccessor));
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<Pager>();
        }
    }
}