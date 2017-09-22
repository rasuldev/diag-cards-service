using System;
using System.Collections.Generic;
using CsQuery.EquationParser.Implementation;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace WebUI.Infrastructure
{
    public static class MessagesHelper
    {
        private const string InfoKey = "MessageText";
        private const string ErrorKey = "ErrorText";

        private static List<string> GetMessages(ITempDataDictionary tempData, string key)
        {
            return new List<string>(tempData[key] as IEnumerable<string> ?? new string[0]);
        }
        private static void AddMessage(this ITempDataDictionary tempData, string key, string text)
        {
            var messages = GetMessages(tempData, key);
            messages.Add(text);
            tempData[key] = messages;
        }
        public static void AddInfoMessage(this Controller controller, string text)
        {
            AddMessage(controller.TempData, InfoKey, text);
        }
        public static void AddErrorMessage(this Controller controller, string text)
        {
            AddMessage(controller.TempData, ErrorKey, text);
        }
        public static void AddInfoMessage(this ITempDataDictionary tempData, string text)
        {
            AddMessage(tempData, InfoKey, text);
        }
        public static void AddErrorMessage(this ITempDataDictionary tempData, string text)
        {
            AddMessage(tempData, ErrorKey, text);
        }
        public static IHtmlContent ShowInfo(this IHtmlHelper helper, object text)
        {
            return ShowMessage(helper, "info", text as string);
        }

        public static IHtmlContent ShowError(this IHtmlHelper helper, object text)
        {
            return ShowMessage(helper, "danger", text as string);
        }

        private static IHtmlContent ShowMessages(this IHtmlHelper helper, string key)
        {
            Func<string, IHtmlContent> showMessage;
            if (key == InfoKey)
                showMessage = t => ShowInfo(helper, t);
            else
                showMessage = t => ShowError(helper, t);

            var messages = GetMessages(helper.TempData, key);
            //var builder = new TagBuilder("");
            var builder = new HtmlContentBuilder();
            foreach (var message in messages)
            {
                builder.AppendHtml(showMessage(message));
            }
            return builder;
        }

        public static IHtmlContent ShowErrorMessages(this IHtmlHelper helper)
        {
            return ShowMessages(helper, ErrorKey);
        }

        public static IHtmlContent ShowInfoMessages(this IHtmlHelper helper)
        {
            return ShowMessages(helper, InfoKey);
        }


        private static IHtmlContent ShowMessage(this IHtmlHelper helper, string blockType, string text)
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