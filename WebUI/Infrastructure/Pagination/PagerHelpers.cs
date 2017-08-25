using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace WebUI.Infrastructure.Pagination
{
    public static class PagerHelpers
    {
        public static IHtmlContent PagerLinks(this IHtmlHelper helper, int totalCount, int pageSize = 10, int visiblePagesCount = 10)
        {
            var pager = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<Pager>();
            return pager.GetHtml(totalCount, pageSize, visiblePagesCount);
        }
    }
}