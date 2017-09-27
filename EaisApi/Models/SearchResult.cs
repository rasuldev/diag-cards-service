using System;

namespace EaisApi.Models
{
    public class SearchResult
    {
        public string this[int index]
        {
            set
            {
                switch (index)
                {
                    case 0:
                        BlankNumber = value;
                        break;
                    case 1:
                        DocumentType = value;
                        break;
                    case 2:
                        IssueDate = value;
                        break;
                    case 3:
                        Vin = value;
                        break;
                    case 4:
                        BodyNumber = value;
                        break;
                    case 5:
                        FrameNumber = value;
                        break;
                    case 6:
                        RegNumber = value;
                        break;
                    case 7:
                        ExpirationDate = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public string BlankNumber { get; set; }
        public string DocumentType { get; set; }
        public string IssueDate { get; set; }
        public string Vin { get; set; }
        /// <summary>
        /// Кузов
        /// </summary>
        public string BodyNumber { get; set; }
        /// <summary>
        /// Шасси
        /// </summary>
        public string FrameNumber { get; set; }
        /// <summary>
        /// Регзнак
        /// </summary>
        public string RegNumber { get; set; }
        public string ExpirationDate { get; set; }
    }
}