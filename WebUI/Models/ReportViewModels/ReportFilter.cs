using System;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Models.ReportViewModels
{
    public class ReportFilter
    {
        //[Required(ErrorMessage = "Required"), Display(Name = "'Дата регистрации: от'")]
        [DataType(DataType.Text)]
        public DateTime? Start { get; set; }
        //[Required(ErrorMessage = "Required"), Display(Name = "'Дата регистрации: до'")]
        [DataType(DataType.Text)]
        public DateTime? End { get; set; }
    }
}