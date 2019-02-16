using System.ComponentModel.DataAnnotations;

namespace WebUI.Data.Entities
{
    public class EaistoCredential
    {
        public int Id { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
    }
}