using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebUI.Infrastructure
{
    public static class MessagesHelper
    {
        public static IHtmlContent ShowInfo(this IHtmlHelper helper, object text)
        {
            return ShowText(helper, "info", text as string);
        }

        public static IHtmlContent ShowError(this IHtmlHelper helper, object text)
        {
            return ShowText(helper, "danger", text as string);
        }

        private static IHtmlContent ShowText(this IHtmlHelper helper, string blockType, string text)
        {
            if (string.IsNullOrEmpty(text))
                return HtmlString.Empty;
            var div = new TagBuilder("div");
            div.AddCssClass("alert");
            div.AddCssClass($"alert-{blockType}");
            div.InnerHtml.SetContent(text);
            return div;
        }
    }
}