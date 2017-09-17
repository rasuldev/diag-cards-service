using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Логин/Email")]
        //[EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }
        [Display(Name = "Введите текст с картинки")]
        public string CaptchaText { get; set; }
    }
}
