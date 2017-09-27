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
            using (var fs = new FileStream(_templateFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                await fs.CopyToAsync(memoryStream);
            }
            memoryStream.Position = 0;
            using (var docx = DocX.Load(memoryStream))
            {
                void Replace(string oldVal, string newVal) => docx.ReplaceText(oldVal, newVal?.ToUpper() ?? "");
                // Replaces text #<start>#, #<start+1>#,..., "<end>" with chars from value
                void ReplaceSeq(int start, int end, string value)
                {
                    if (string.IsNullOrEmpty(value)) return;
                    for (int i = start; i <= end; i++)
                    {
                        Replace($"#{i}#", value[i - start].ToString());
                    }
                }

                ReplaceSeq(1, 15, card.CardId);
                ReplaceSeq(20, 27, card.ExpirationDate?.ToString("ddMMyyyy"));
                ReplaceSeq(30, 37, card.RegisteredDate?.ToString("ddMMyyyy"));
                Replace("#reg_znak#", card.RegNumber);
                Replace("#vin#", card.VIN);
                Replace("#rama#", card.FrameNumber);
                Replace("#kuzov#", card.BodyNumber);
                Replace("#vidano#", $"{card.DocumentSeries} {card.DocumentNumber} {card.DocumentIssuer} {card.DocumentIssueDate:dd.MM.yyyy}");
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