using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebUI.Models;

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
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow.AddHours(3);
        /// <summary>
        /// Saved in Eaisto date
        /// </summary>
        [Display(Name = "Дата выдачи карты")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? RegisteredDate { get; set; }

        [Display(Name = "Тип")]
        public CardTypes CardType { get; set; }

        [NotMapped]
        [Display(Name = "ФИО")]
        public string Fullname => $"{Lastname} {Firstname} {Patronymic}";

        [NotMapped]
        public bool IsRegistered => RegisteredDate != null;
    }
}