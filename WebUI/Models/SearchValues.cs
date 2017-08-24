using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Models
{
    public class SearchValues
    {
        [Display(Name = "RegNumber")]
        [RegularExpression(@"[А-Я]\d{3}[А-Я]{2}\d{2}", ErrorMessage = "Введите корректный регистрационный номер")]
        public string RegNumber { get; set; }

        [Display(Name = "VIN")]
        [RegularExpression(@"[А-ЯA-Z0-9]", ErrorMessage = "Введите корректный VIN")]
        public string VIN { get; set; }

        [Display(Name = "FIO")]
        [RegularExpression(@"[а-яА-Яa-zA-Z^0-9]*", ErrorMessage = "Введите корректное имя")]
        public string FIO { get; set; }

        [Display(Name = "StartDate")]
        [DataType(DataType.Date, ErrorMessage = "Укажите корректную дату")]
        public DateTime StartDate = new DateTime(1, 1, 1);

        [Display(Name = "EndDate")]
        [DataType(DataType.Date, ErrorMessage = "Укажите корректную дату")]
        public DateTime EndDate = DateTime.Now;
    }
}
