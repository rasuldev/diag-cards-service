using System;

namespace WebUI.Models.CardsViewModels
{
    public class FilterCardsParams
    {
        public SortParamEnum SortBy { get; set; }
        public string Regnumber { get; set; }
        public string Vin { get; set; }
        public string Fullname { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public CardStatusEnum? Status { get; set; }
        public string UserId { get; set; }
    }
}