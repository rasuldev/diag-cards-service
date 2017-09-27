using System.Collections.Generic;
using EaisApi.Models;

namespace WebUI.Models.CardsViewModels
{
    public class SearchEaistoModel
    {
        public List<SearchResult> Results { get; set; }
        public SearchEaistoFilter Filter { get; set; }
    }
}