using System;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Models.ReportViewModels
{
    public class ReportFilter
    {
        //[Required(ErrorMessage = "Required"), Display(Name = "'Дата регистрации: от'")]
        public DateTime? Start { get; set; }
        //[Required(ErrorMessage = "Required"), Display(Name = "'Дата регистрации: до'")]
        public DateTime? End { get; set; }
    }
}