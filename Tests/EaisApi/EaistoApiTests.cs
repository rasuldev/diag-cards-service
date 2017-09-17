using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using EaisApi;
using EaisApi.Exceptions;
using EaisApi.Models;
using Newtonsoft.Json;
using NUnit.Framework;


namespace Tests.EaisApi
{
    public class EaistoApiTests
    {
        private EaistoApi _api;

        [OneTimeSetUp]
        public async Task Init()
        {
            _api = await SignIn();
        }

        private async Task<EaistoApi> SignIn()
        {
            var api = new EaistoApi(new MemoryStorage());
            var captcha = await api.InitRemoteSession();
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".jpg");
            using (var file = new FileStream(path, FileMode.Create))
            {
                captcha.CopyTo(file);
                captcha.Dispose();
            }
            //Process.Start(path);
            Process.Start(new ProcessStartInfo("cmd", $"/c start {path}") { CreateNoWindow = true });
            var code = Console.ReadLine();
            File.Delete(path);
            await api.SignIn("login", "pass", code);
            return api;
        }

        //[Test]
        //public async Task SignInTest()
        //{
        //    await SignIn();
        //}

        [Test]
        public async Task GetRunningInfoTest()
        {
            var info = await _api.GetVehicleRunning("12345678912345678");
            Assert.AreEqual(123456, info.Running);
            Assert.AreEqual(new DateTime(2017, 07, 12), info.Date);
        }

        [Test]
        public void JsonTest()
        {
            //var json = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>("[{\"PROBEG\":\"123456\",\"DATA\":\"20170712000000\"}]");
            //Assert.AreEqual("123456", json[0]["PROBEG"]);
            var json = JsonConvert.DeserializeObject<List<VehicleRunningInfo>>("[{\"PROBEG\":\"123456\",\"DATA\":\"20170712000000\"}]", new JsonSerializerSettings()
            {
                DateFormatString = "yyyyMMdd000000"
            });
            Assert.AreEqual(123456, json[0].Running);
        }

        [Test]
        public async Task GetManufacturersTest()
        {
            var json = await _api.GetManufacturers("ва");
            Assert.That(json.Contains("ВАЗ"));
        }

        [Test]
        public async Task GetModelsTest()
        {
            var json = await _api.GetModels("ваз", "21");
            Assert.That(json.Contains("21"));
        }

        [Test]
        public async Task SessionExpiredTest()
        {
            var storage = new MemoryStorage();
            storage.SaveData(new ApiUserData("", ""));
            var api = new EaistoApi(storage);
            Assert.ThrowsAsync<NotAuthorizedException>(async () => await api.Search(regNumber: "111"));
        }

        [Test]
        public async Task SaveCardTest()
        {
            try
            {
                var info = new DiagnosticCard()
                {
                    Lastname = "ТестМагомедов",
                    Firstname = "ТестМагомед",
                    Patronymic = "ТестМагомедович",
                    VIN = "12345678912345672",
                    IssueYear = 2012,
                    Manufacturer = "Газ",
                    Model = "53",
                    FrameNumber = "12345678912345678",
                    Running = 250000,
                    RegNumber = "Т111СТ05",
                    Weight = 2000,
                    Category = VehicleCategory.B,
                    CategoryCommon = VehicleCategoryCommon.N1,
                    TyreManufacturer = "Kama",
                    AllowedMaxWeight = 3000,
                    FuelType = FuelTypes.Diesel,
                    BrakeType = BrakeTypes.Hydraulic,
                    DocumentType = DocumentTypes.RegistrationCertificate,
                    DocumentSeries = "0123",
                    DocumentNumber = "123456",
                    DocumentIssueDate = new DateTime(2017, 02, 01),
                    DocumentIssuer = "ГИБДД",
                    Note = "Тестовое примечание",
                    ExpirationDate = new DateTime(2018, 08, 09),
                };
                var id = await _api.SaveCard(info);
                Assert.NotNull(id);
                Assert.That(id, Does.Match(@"\d+"));
            }
            catch (CheckCardException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
