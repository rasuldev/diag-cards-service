using System.IO;
using System.Threading.Tasks;
using Novacode;
using WebUI.Data.Entities;

namespace WebUI.Infrastructure
{
    public class CardDocxGenerator
    {
        private readonly string _templateFile;

        public CardDocxGenerator(string templateFile)
        {
            _templateFile = templateFile;
        }

        public async Task<Stream> Generate(DiagnosticCard card)
        {
            var memoryStream = new MemoryStream();
            using (var fs = new FileStream(_templateFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await fs.CopyToAsync(memoryStream);
            }
            memoryStream.Position = 0;
            using (var docx = DocX.Load(memoryStream))
            {
                void Replace(string oldVal, string newVal) => docx.ReplaceText(oldVal, newVal?.ToUpper() ?? "");
                Replace("#reg_znak#", card.RegNumber);
                Replace("#vin#", card.VIN);
                Replace("#rama#", card.FrameNumber);
                Replace("#kuzov#", card.BodyNumber);
                Replace("#vidano#", $"{card.DocumentSeries}, {card.DocumentNumber}, выдан {card.DocumentIssuer} {card.DocumentIssueDate:dd.MM.yyyy}");
                Replace("#marka#", $"{card.Manufacturer} {card.Model}");
                Replace("#kategor1#", card.Category.ToString());
                Replace("#god_vipuska#", card.IssueYear.ToString());
                Replace("#massa_bez#", card.Weight.ToString());
                Replace("#massa_max#", card.AllowedMaxWeight.ToString());
                Replace("#toplivo#", card.FuelType.Label());
                Replace("#probeg#", card.Running.ToString());
                Replace("#tormozn_system#", card.BrakeType.Label().Split('-')[0].Trim());
                Replace("#marka_shin#", card.TyreManufacturer);
                Replace("#notes#", card.Note);
                docx.Save();
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}