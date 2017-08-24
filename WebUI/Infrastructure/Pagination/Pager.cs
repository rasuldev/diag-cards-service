using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods.Internal;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Internal.Networking;
using Microsoft.AspNetCore.WebUtilities;

namespace WebUI.Infrastructure.Pagination
{
    /// <summary>
    /// 1. Place in startup.cs the following line:
    /// services.AddPager();
    /// 2. Add to _ViewImports 
    /// @using WebUI.Infrastructure.Pagination
    /// 3. In View use
    /// @Html.PagerLinks(itemsCount,...)
    /// 4. To get current page number request service Pager and use it CurrentPage property.
    /// </summary>
    public class Pager
    {
        private readonly string _actionName;
        private readonly RouteValueDictionary _routeValues;
        private readonly UrlHelper _helper;

        public int CurrentPage { get; private set; }
        //public int PageSize { get; set; } = 10;
        //public int VisiblePagesCount { get; set; } = 10;
        //public int TotalCount { get; set; }
        //public int PagesCount => TotalCount / PageSize + 1;
        public string PageUrlParam { get; set; } = "page";
        public string Css { get; set; } = "pager";
        public string PageCss { get; set; } = "page";
        public string CurrentPageCss { get; set; } = "selected";
        public string NextSetTextOn { get; set; } = "...";
        public string PreviousSetTextOn { get; set; } = "...";
        public string NextSetTextOff { get; set; } = "";
        public string PreviousSetTextOff { get; set; } = "";
        public string NextSetCssOn { get; set; } = "next-set";
        public string PreviousSetCssOn { get; set; } = "prev-set";
        public string NextSetCssOff { get; set; } = "next-set-off";
        public string PreviousSetCssOff { get; set; } = "prev-set-off";
        public string EmptyPagerHtml { get; set; } = "";

        public Pager(IActionContextAccessor actionContextAccessor)
        {
            _helper = new UrlHelper(actionContextAccessor.ActionContext);
            _routeValues = new RouteValueDictionary(actionContextAccessor.ActionContext.HttpContext.GetRouteData().Values);
            _actionName = _routeValues["action"].ToString();
            
            foreach (var queryParam in actionContextAccessor.ActionContext.HttpContext.Request.Query)
            {
                _routeValues.Add(queryParam.Key, queryParam.Value);
            }

            
            //var s = QueryHelpers.AddQueryString(actionContextAccessor.ActionContext.HttpContext.Request.GetUri().ToString(),
            //    PageUrlParam, "5");

            CurrentPage = 1;
            if (_routeValues.ContainsKey(PageUrlParam))
                CurrentPage = int.Parse(_routeValues[PageUrlParam].ToString());
            if (actionContextAccessor.ActionContext.HttpContext.Request.Query.ContainsKey(PageUrlParam))
                CurrentPage = int.Parse(actionContextAccessor.ActionContext.HttpContext.Request.Query[PageUrlParam].ToString());
        }

        private string PageUrl(int pageNumber)
        {
            _routeValues[PageUrlParam] = pageNumber;
            return _helper.Action(_actionName, _routeValues);
        }

        public IHtmlContent GetHtml(int totalCount, int pageSize = 10, int visiblePagesCount = 10)
        {
            if (totalCount == 0) return new HtmlString(EmptyPagerHtml);
            var pagesCount = (totalCount - 1) / pageSize + 1;
            if (CurrentPage < 1)
                CurrentPage = 1;
            if (CurrentPage > pagesCount)
                CurrentPage = pagesCount;

            TagBuilder tag;

            var ul = new TagBuilder("ul");
            
            ul.AddCssClass(Css);

            var sb = new StringBuilder();
            //...56 57 58 59 60...
            // calc first visible page number (in example above it would be 56)
            var beginPageNumber = CurrentPage - ((CurrentPage - 1) % visiblePagesCount);

            // if first number is greater than 1, we should show link to previous set of page numbers
            if (beginPageNumber > 1)
            {
                tag = new TagBuilder("a");
                tag.AddCssClass(PreviousSetCssOn);
                tag.MergeAttribute("href", PageUrl(beginPageNumber - 1));
                tag.InnerHtml.SetContent(PreviousSetTextOn);
                var li = new TagBuilder("li");
                li.InnerHtml.SetHtmlContent(tag);
                ul.InnerHtml.AppendHtml(li);
            }
            else
            {
                tag = new TagBuilder("li");
                tag.AddCssClass(PreviousSetCssOff);
                tag.InnerHtml.SetContent(PreviousSetTextOff);
                //sb.Append(tag);
                ul.InnerHtml.AppendHtml(tag);
            }
            var lastPageNumber = Math.Min(beginPageNumber + visiblePagesCount - 1, pagesCount);
            for (var i = beginPageNumber; i <= lastPageNumber; i++)
            {
                tag = new TagBuilder("a");
                tag.MergeAttribute("href", PageUrl(i));
                tag.InnerHtml.SetContent(i.ToString());
                tag.AddCssClass(i != CurrentPage ? PageCss : CurrentPageCss);
                var li = new TagBuilder("li");
                li.InnerHtml.SetHtmlContent(tag);
                ul.InnerHtml.AppendHtml(li);
            }

            if (lastPageNumber < pagesCount)
            {
                tag = new TagBuilder("a");
                tag.AddCssClass(NextSetCssOn);
                tag.MergeAttribute("href", PageUrl(lastPageNumber + 1));
                tag.InnerHtml.SetContent(PreviousSetTextOn);
                var li = new TagBuilder("li");
                li.InnerHtml.SetHtmlContent(tag);
                ul.InnerHtml.AppendHtml(li);
            }
            else
            {
                tag = new TagBuilder("li");
                tag.AddCssClass(NextSetCssOff);
                tag.InnerHtml.SetContent(PreviousSetTextOff);
                sb.Append(tag);
            }

            return ul;
        }

        
    }
}