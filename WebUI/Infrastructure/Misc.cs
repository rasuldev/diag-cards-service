using System;
using System.Collections.Generic;
using System.Linq;
using CsQuery.ExtensionMethods.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebUI.Infrastructure
{
    public static class Misc
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selectedValue"></param>
        /// <param name="emptyOption">Adds first option to select with empty value. If it is null, no option will be added</param>
        /// <returns></returns>
        public static SelectList CreateSelectListFrom<T>(Enum selectedValue = null, string emptyOption = null)
        {
            var enumType = typeof(T);
            if (selectedValue != null && selectedValue.GetType() != enumType)
                throw new ArgumentException("selectedValue argument must have type enumType");
            var items = new List<SelectListItem>();
            if (emptyOption != null)
                items.Add(new SelectListItem()
                {
                    Value = "",
                    Text = emptyOption
                });
            items.AddRange(Enum.GetValues(enumType).Cast<Enum>()
                .Select(v => new SelectListItem()
                {
                    Text = v.Label(),
                    Value = v.GetValue().ToString()
                }));
            return new SelectList(items, "Value", "Text", selectedValue?.GetValueAsString());
        }

        public static string ToRussianDateFormat(this DateTime? date)
        {
            return date?.ToRussianDateFormat();
        }

        public static string ToRussianDateFormat(this DateTime date)
        {
            return date.ToString("dd.MM.yyyy");
        }
    }

    //public struct SelectListItem
    //{
    //    public string Value { get; set; }
    //    public string Text { get; set; }

    //    public SelectListItem(string value, string text)
    //    {
    //        Value = value;
    //        Text = text;
    //    }
    //}
}