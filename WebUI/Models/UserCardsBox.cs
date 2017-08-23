using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebUI.Data.Entities;

namespace WebUI.Models
{
    public class UserCardsBox
    {
        public List<DiagnosticCard> RegisteredCards { get; set; }
        public List<DiagnosticCard> NotRegisteredCards { get; set; }
    }
}
