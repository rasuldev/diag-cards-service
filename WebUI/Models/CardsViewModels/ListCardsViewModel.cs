using System.Collections.Generic;
using WebUI.Data.Entities;

namespace WebUI.Models.CardsViewModels
{
    public class ListCardsViewModel
    {
        public List<DiagnosticCard> Cards { get; set; }
        public int TotalCardsCount { get; set; }
        public FilterCardsParams Filter { get; set; } = new FilterCardsParams();
    }
}