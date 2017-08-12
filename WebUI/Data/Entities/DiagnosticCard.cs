﻿using System.ComponentModel.DataAnnotations.Schema;

namespace WebUI.Data.Entities
{
    public class DiagnosticCard: EaisApi.Models.DiagnosticCard
    {
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}