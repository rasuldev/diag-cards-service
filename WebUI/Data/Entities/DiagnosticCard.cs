using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebUI.Data.Entities
{
    public class DiagnosticCard: EaisApi.Models.DiagnosticCard
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        /// <summary>
        /// Saved in Eaisto date
        /// </summary>
        public DateTime? RegisteredDate { get; set; }
    }
}