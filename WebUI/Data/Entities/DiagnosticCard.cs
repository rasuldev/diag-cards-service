using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebUI.Data.Entities
{
    public class DiagnosticCard: EaisApi.Models.DiagnosticCard
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [Display(Name = "Пользователь")]
        public User User { get; set; }
        [Display(Name = "Дата создания")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        /// <summary>
        /// Saved in Eaisto date
        /// </summary>
        [Display(Name = "Дата регистрации")]
        public DateTime? RegisteredDate { get; set; }

        [NotMapped]
        [Display(Name = "ФИО")]
        public string Fullname => $"{Lastname} {Firstname} {Patronymic}";

        [NotMapped]
        public bool IsRegistered => RegisteredDate != null;
    }
}