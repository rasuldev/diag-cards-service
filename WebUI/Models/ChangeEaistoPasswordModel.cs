using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class ChangeEaistoPasswordModel
    {
        [Required]
        [Display(Name = "Админский логин")]
        public string LocalLogin { get; set; }
        [Required]
        [Display(Name = "Админский пароль")]
        public string LocalPassword { get; set; }
        [Required]
        [Display(Name = "Пароль на еаисто")]
        public string EaistoPassword { get; set; }
    }
}