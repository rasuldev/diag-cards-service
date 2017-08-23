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
        public User User { get; set; }
        [Display(Name = "Дата создания")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        /// <summary>
        /// Saved in Eaisto date
        /// </summary>
        [Display(Name = "Дата регистрации")]
        public DateTime? RegisteredDate { get; set; }
    }
}