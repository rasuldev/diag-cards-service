using System;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Models.CardsViewModels
{
    public class FilterCardsParams
    {
        public SortParamEnum SortBy { get; set; }
        public string Regnumber { get; set; }
        public string Vin { get; set; }
        public string Fullname { get; set; }
        [DataType(DataType.Text)]
        public DateTime? StartDate { get; set; }
        [DataType(DataType.Text)]
        public DateTime? EndDate { get; set; }
        public CardStatusEnum? Status { get; set; }
        public string UserId { get; set; }
    }
}