namespace EaisApi.Models
{
    public class SearchParams
    {
        /// <summary>
        /// ВИН
        /// </summary>
        public string Vin {get; set;}
        /// <summary>
        /// Регистрационный номер
        /// </summary>
        public string RegNumber {get; set;}
        /// <summary>
        /// Серия талона ТО
        /// </summary>
        public string TicketSeries {get; set;}
        /// <summary>
        /// Номер талона ТО(номер ДК)
        /// </summary>
        public string TicketNumber {get; set;}
        /// <summary>
        /// Номер кузова
        /// </summary>
        public string BodyNumber {get; set;}
        /// <summary>
        /// Номер шасси (рамы)
        /// </summary>
        public string FrameNumber;
    }
}