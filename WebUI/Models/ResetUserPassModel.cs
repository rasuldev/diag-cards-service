using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class ResetUserPassModel
    {
        [Required]
        public string UserId { get; set; }

        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Длина пароля должна быть от {2} до {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Введите новый пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }
    }
}