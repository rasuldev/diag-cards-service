using System.Collections.Generic;

namespace EaisApi.Models
{
    public class ManufacturerSuggestions
    {
        public string Query { get; set; }
        public List<string> Suggestions { get; set; }
        public List<int> Data { get; set; }
    }
}