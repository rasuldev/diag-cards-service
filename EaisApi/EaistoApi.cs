﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CsQuery;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Utility;
using EaisApi.Exceptions;
using EaisApi.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EaisApi
{
    /// <summary>
    /// Usage:
    /// 
    /// Call InitRemoteSession first. This method returns captcha. 
    /// Ask user to resolve this captcha. Then use SignIn method. 
    /// If no exception was raised, then you can use SaveToService.
    /// </summary>
    public partial class EaistoApi
    {
        /// <summary>
        /// User storage is used for persisting remote session cookie (PHPSESSID).
        /// This storage should have per user behavour (each user has each own storage).
        /// </summary>
        private readonly IUserStorage _userStorage;
        private const string LoginUrl = "https://eaisto.gibdd.ru/ru/arm/";
        private const string CaptchaUrl = "https://eaisto.gibdd.ru/common/tool/getcaptcha.php?captcha_id=";
        private const string DataUrl = "https://eaisto.gibdd.ru/ru/arm/expert/new/";
        private const string SearchUrl = "https://eaisto.gibdd.ru/ru/arm/expert/dublikat_poisk/";
        private const string RunningUrl = "https://eaisto.gibdd.ru/ru/arm/expert/new/?get_diag_karta_probeg=";
        private const string SearchManufacturersUrl = "https://eaisto.gibdd.ru/ru/arm/expert/new/?get_marks=1&query=";
        private const string SearchModelsUrl = "https://eaisto.gibdd.ru/ru/arm/expert/new/?get_models=1&marka=$man$&query=";
        private static HttpClient Client;
        private readonly ILogger<EaistoApi> _logger;

        //public string SessionId { get; set; }
        public static void SetHttpClient(HttpClient client)
        {
            Client = client;
        }
        static EaistoApi()
        {
            //var handler = new HttpClientHandler
            //{
            //    AllowAutoRedirect = false,
            //    UseCookies = false,
            //    //CookieContainer = cookies
            //};

            //Client = new HttpClient(handler);
        }

        public EaistoApi(IUserStorage storage, ILogger<EaistoApi> logger)
        {
            _userStorage = storage;
            _logger = logger;
        }

        /// <summary>
        /// Sends request to login page, saves session cookie and returns captcha image
        /// </summary>
        /// <returns></returns>
        public async Task<Stream> InitRemoteSession()
        {
            //try
            //{
            //    //var res = await Client.GetStringAsync("https://tooba.site");
            //    //var res = await Client.GetStringAsync("http://ya.ru");
            //    var res = await Client.GetStringAsync(LoginUrl);
            //    //_logger.LogInformation("Response from ya.ru" + res);
            //}
            //catch (Exception e)
            //{
            //    _logger.LogError(e.ToString());
            //}

            var result = await Client.GetAsync(LoginUrl);

            if (!result.Headers.TryGetValues("Set-cookie", out var cookies))
            {
                throw new CookieNotFoundException("Session cookie is not found.");
            }
            var dom = CQ.Create(await result.Content.ReadAsStringAsync());
            var captchaId = dom["img[name=captcha]"].Attr("id");
            // save session cookie and captcha id to user storage
            await _userStorage.SaveData(new ApiUserData(string.Join(";", cookies), captchaId));
            //var captchaUrl = "http://eaisto.gibdd.ru"+dom["img[name=captcha]"].Attr("src");
            //var request = new HttpRequestMessage(HttpMethod.Get, LoginUrl);
            //var captcha = Console.ReadLine();
            return await GetCaptcha(captchaId);
        }

        private async Task<Stream> GetCaptcha(string captchaId)
        {
            var url = $"{CaptchaUrl}{captchaId}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            await SetSessionCookie(request);
            var result = await Client.SendAsync(request);

            //if (result.Headers.TryGetValues("Set-cookie", out var cookies))
            //{
            //    // we don't expect new cookie
            //    throw new Exception("Session restarted");
            //}
            return await result.Content.ReadAsStreamAsync();
            //using (var filestream = new FileStream(Path.Combine(_captchaFolder, GenerateUniqueFilename()),FileMode.Create))
            //{
            //    await result.Content.CopyToAsync(filestream);
            //}
        }

        /// <summary>
        /// It should be used for prolong session on EAISTO.
        /// </summary>
        /// <returns></returns>
        public async Task<string> ProlongSession()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, DataUrl);
            await SetSessionCookie(request);
            var response = await Client.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
                _logger.LogError($"Eaisto prolong session error: status={response.StatusCode}; request");
            return response.StatusCode.ToString();
        }
        
        public async Task<bool> IsSessionExpired()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, DataUrl);
                await SetSessionCookie(request);
                var response = await Client.SendAsync(request);
                return sessionIsExpired(response);
            }
            catch (NotAuthorizedException)
            {
                return true;
            }
        }

        //private static string GenerateUniqueFilename()
        //{
        //    //var name = Path.GetFileNameWithoutExtension(filename);
        //    return $"captcha_{DateTime.Now:yyyy-MM-ddTHH-mm-ss-ff}_{Path.GetRandomFileName().Replace(".", "")}.jpg";
        //}


        //private void SetSessionCookie(HttpRequestMessage request, string sessionId)
        //{
        //    request.Headers.Add("Cookie", $"{SessionKey}={sessionId}");
        //}

        private async Task SetSessionCookie(HttpRequestMessage request)
        {
            var userData = await _userStorage.LoadData();
            if (userData == null)
            {
                _logger.LogError($"User data is null.");
                throw new NotAuthorizedException("Session cookie is missing in user storage");
            }
            request.Headers.Add("Cookie", userData.Cookies);
        }

        public async Task SignIn(string login, string password, string captcha)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, LoginUrl);
            await SetSessionCookie(request);
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("login", login),
                new KeyValuePair<string, string>("pass", password),
                new KeyValuePair<string, string>("check_3", captcha),
                new KeyValuePair<string, string>("captcha_id_3", (await _userStorage.LoadData()).CaptchaId)
            });
            request.Content = content;
            var result = await Client.SendAsync(request);
            //var contentStr = await result.Content.ReadAsStringAsync();
            var location = result.Headers.Location;
            // on success we should be redirected. If no, check for wrong captcha
            if (location == null)
            {
                //var dom = CQ.Create(await result.Content.ReadAsStringAsync());
                //dom["#form_element div[name*='color:red'"].Html()
                throw new WrongCaptchaException("Sign in error: wrong captcha");
            }
            //var html = await result.Content.ReadAsStringAsync();
            //Console.WriteLine(html);
        }


        // search[VIN]	
        //search[REG_ZNAK]
        //search[SERIA]
        //search[NOMER]
        //search[NOMER_KUZOVA]
        //search[NOMER_RAMY]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vin">ВИН</param>
        /// <param name="regNumber">Регистрационный номер авто</param>
        /// <param name="ticketSeries">Серия талона ТО</param>
        /// <param name="ticketNumber">Номер талона ТО (номер ДК)</param>
        /// <param name="bodyNumber">Номер кузова</param>
        /// <param name="frameNumber">Номер шасси (рамы)</param>
        /// <returns></returns>
        public async Task<List<SearchResult>> Search(string vin = null, string regNumber = null, string ticketSeries = null,
            string ticketNumber = null, string bodyNumber = null, string frameNumber = null)
        {
            string Escape(string s) => Uri.EscapeUriString(s ?? "");
            var query = new StringBuilder()
                .Append($"?search[VIN]={Escape(vin)}&search[REG_ZNAK]={Escape(regNumber)}&search[SERIA]={Escape(ticketSeries)}")
                .Append($"&search[NOMER]={Escape(ticketNumber)}&search[NOMER_KUZOVA]={Escape(bodyNumber)}&search[NOMER_RAMY]={Escape(frameNumber)}");
            var request = new HttpRequestMessage(HttpMethod.Get, SearchUrl + query);
            await SetSessionCookie(request);
            var response = await Client.SendAsync(request);
            if (sessionIsExpired(response))
            {
                throw new NotAuthorizedException();
            }
            var html = await response.Content.ReadAsStringAsync();
            var cq = CQ.Create(html);

            var rows = cq[".white_tbl tr.td_white"];
            var results = new List<SearchResult>();
            foreach (var row in rows)
            {
                var columns = CQ.Create(row.InnerHTML)["td"].Skip(1).Take(8).ToList();
                var search = new SearchResult();
                for (int i = 0; i < 8; i++)
                {
                    search[i] = columns[i].InnerText;
                }
                results.Add(search);
            }
            return results;
        }

        //public async Task<string> Search(SearchParams search)
        //{
        //    await Search(search.Vin,search.RegNumber,search.TicketSeries, )
        //}

        private bool sessionIsExpired(HttpResponseMessage response)
        {
            var isExpired = response.Headers.Location != null && response.Headers.Location.OriginalString.Contains("from_url");
            if (isExpired)
                _logger.LogInformation("Eaisto session is expired");
            return isExpired;
        }

        public async Task<VehicleRunningInfo> GetVehicleRunning(string vin)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, RunningUrl + Uri.EscapeUriString(vin));
            await SetSessionCookie(request);
            var response = await Client.SendAsync(request);
            if (sessionIsExpired(response))
            {
                throw new NotAuthorizedException();
            }
            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json.Trim()) || json == "[]")
                return null;
            var info = JsonConvert.DeserializeObject<List<VehicleRunningInfo>>(json, new JsonSerializerSettings()
            {
                DateFormatString = "yyyyMMddhhmmss"
            });

            return info.FirstOrDefault();
        }

        public async Task<string> GetManufacturers(string query)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, SearchManufacturersUrl + Uri.EscapeUriString(query));
            await SetSessionCookie(request);
            var response = await Client.SendAsync(request);
            if (sessionIsExpired(response))
            {
                throw new NotAuthorizedException();
            }
            //var bytes = await response.Content.ReadAsByteArrayAsync();
            var json = await response.Content.ReadAsStringAsync();
            json = System.Text.RegularExpressions.Regex.Unescape(json);
            return json;
        }

        public async Task<string> GetAllManufacturers()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, SearchManufacturersUrl);
            await SetSessionCookie(request);
            var response = await Client.SendAsync(request);
            if (sessionIsExpired(response))
            {
                throw new NotAuthorizedException();
            }
            //var bytes = await response.Content.ReadAsByteArrayAsync();
            var json = await response.Content.ReadAsStringAsync();
            json = System.Text.RegularExpressions.Regex.Unescape(json);
            return json;
        }

        public async Task<string> GetModels(string manufacturer, string query)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                SearchModelsUrl.Replace("$man$", Uri.EscapeUriString(manufacturer)) + Uri.EscapeUriString(query));
            await SetSessionCookie(request);
            var response = await Client.SendAsync(request);
            if (sessionIsExpired(response))
            {
                throw new NotAuthorizedException();
            }
            var json = await response.Content.ReadAsStringAsync();
            json = System.Text.RegularExpressions.Regex.Unescape(json);
            return json;
        }

        /// <summary>
        /// Saves card and sets it card id. 
        /// Throws CheckCardException if form didn't pass validation on server.
        /// See <see cref="CheckResults"/> enum for details.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task SaveCard(DiagnosticCard info)
        {
            // check form
            var checkResult = await CheckForm(info);
            if (checkResult != CheckResults.Success)
            {
                if (checkResult == CheckResults.DuplicateToday)
                {
                    // it means that card has been registered
                    // so we try to get card info using search
                    if (await RefreshCardFromEaistoSearch(info))
                        return;
                }
                throw new CheckCardException(checkResult);
            }

            var request = new HttpRequestMessage(HttpMethod.Post, DataUrl);
            await SetSessionCookie(request);

            var formParams = new List<KeyValuePair<string, string>>();
            PopulateParams(formParams, info);

            var content = new FormUrlEncodedContent(formParams);
            request.Content = content;

            var response = await Client.SendAsync(request);
            if (sessionIsExpired(response))
            {
                throw new NotAuthorizedException();
            }

            var respContent = await response.Content.ReadAsStringAsync();

            // After post we expect 302 (redirect) response
            var location = response.Headers.Location;
            if (location == null || !location.OriginalString.Contains("expert/new"))
            {
                throw new SaveCardException("Expected redirect response from EAISTO is missing.");
            }

            // Follow redirect to get card id
            request = new HttpRequestMessage(HttpMethod.Get, DataUrl);
            request.Headers.Referrer = new Uri("http://eaisto.gibdd.ru/ru/arm/expert/new/");
            await SetSessionCookie(request);
            response = await Client.SendAsync(request);
            if (sessionIsExpired(response))
            {
                throw new NotAuthorizedException();
            }
            var html = await response.Content.ReadAsStringAsync();
            var cq = CQ.Create(html);
            var cardId = cq[".second_cont strong"].Text();
            if (cardId.Any(c => !Char.IsDigit(c)))
            {
                _logger?.LogError("cardId is wrong. Received html: " + html);
                await RefreshCardFromEaistoSearch(info);
            }

            info.CardId = cardId;
            // jQuery(".second_cont strong")
            // http://eaisto.gibdd.ru/ru/arm/expert/new/

            // Referer	
            // http://eaisto.gibdd.ru/ru/arm/expert/new/index.php?
            //return await result.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Returns true if card is found on eaisto server
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public async Task<bool> RefreshCardFromEaistoSearch(DiagnosticCard card)
        {
            try
            {
                var results = await Search(card.VIN, card.RegNumber);
                var cardInfo = results.SingleOrDefault(c =>
                    c.IssueDate == DateTime.UtcNow.AddHours(3).ToString("dd.MM.yyyy"));
                if (cardInfo != null)
                {
                    card.CardId = cardInfo.BlankNumber.Replace("/", "");
                    card.ExpirationDate = DateTime.ParseExact(cardInfo.ExpirationDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    //card.RegisteredDate = DateTime.ParseExact(cardInfo.ExpirationDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Refresh card error: " + e);
            }
            return false;
        }

        private async Task<CheckResults> CheckForm(DiagnosticCard info)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, DataUrl);
            await SetSessionCookie(request);

            var formParams = new List<KeyValuePair<string, string>>();
            void Add(string key, string value) => formParams.Add(new KeyValuePair<string, string>(key, value));
            Add("check_form", "1");
            Add("vin", info.VIN);
            Add("reg_znak", info.RegNumber);
            Add("kuzov", info.BodyNumber);
            Add("shassi", info.FrameNumber);
            Add("seria", "undefined");
            Add("nomer", "undefined");
            Add("mode", "new");

            var content = new FormUrlEncodedContent(formParams);
            request.Content = content;

            var response = await Client.SendAsync(request);
            if (sessionIsExpired(response))
            {
                throw new NotAuthorizedException();
            }

            var json = await response.Content.ReadAsStringAsync();
            var responseContainer = new { result = -1 };
            return (CheckResults)JsonConvert.DeserializeAnonymousType(json, responseContainer).result;
        }

        private void PopulateParams(List<KeyValuePair<string, string>> formParams, DiagnosticCard info)
        {
            void Add(string key, string value) => formParams.Add(new KeyValuePair<string, string>(key, value));
            Add("diag_karta_id", "");
            Add("F_LICA", info.Lastname);
            Add("I_LICA", info.Firstname);
            Add("O_LICA", info.Patronymic);

            if (info.VIN != null) Add("VIN", info.VIN);
            else Add("VIN_OTSUTSTVUET", "1"); // TODO: check param name
            Add("GOD", info.IssueYear.ToString());
            Add("MARKA", info.Manufacturer);
            Add("MODEL", info.Model);
            Add("NOMER_KUZOVA", info.BodyNumber);
            if (info.FrameNumber != null) Add("NOMER_RAMY", info.FrameNumber);
            else Add("NOMER_RAMY_OTSUTSTVUET", "1");
            Add("PROBEG", info.Running.ToString());
            Add("REG_ZNAK", info.RegNumber);
            Add("MASSA_BEZ_NAGRUZKI", info.Weight.ToString());
            Add("KATEG_SRTS_PTS_ID", info.Category.ToString()); // TODO: check it
            Add("KATEG_ID", info.CategoryCommon.ToString()); // TODO: check it
            Add("MARKA_SHIN", info.TyreManufacturer);
            Add("MARKA_SHIN[0]", info.TyreManufacturer);
            Add("RAZRESH_MAKS_MASSA", info.AllowedMaxWeight.ToString());
            Add("TIP_TOPLIVA", info.FuelType.GetValueAsString());
            Add("TIP_TORMOZ_SISTEMY", info.BrakeType.GetValueAsString());


            Add("SVID_O_REG", info.DocumentType == DocumentTypes.RegistrationCertificate ? "0" : "1");
            Add("SVID_O_REG_SERIA", info.DocumentSeries);
            Add("SVID_O_REG_NOMER", info.DocumentNumber);
            Add("SVID_O_REG_KOGDA", info.DocumentIssueDate?.ToString("dd.MM.yyyy"));
            Add("SVID_O_REG_KEM", info.DocumentIssuer);
            if (info.IsForeigner)
                Add("SVID_FOREIGN", "1");


            // Expertise result (it is not change)
            Add("table_version", "list");
            for (int i = 1; i <= 67; i++)
            {
                Add($"RESULTAT[PROVERKA_NE_PROIDENA][{i}]", "0");
                Add($"RESULTAT[PROVERKA_NE_PROIDENA_DUMMY][{i}]", "1");
            }
            Add("DIAGNOSTIKA[NIZH_GRAN][X]", "");
            Add("DIAGNOSTIKA[ZNACHENIE][X]", "");
            Add("DIAGNOSTIKA[VERH_GRAN][X]", "");
            Add("DIAGNOSTIKA[NAIMENOVANIE][X]", "");
            Add("DIAGNOSTIKA[PARAMETER_ID][X]", "");
            Add("NEVYPOLN[PREDMET_PROVERKI][X]", "");
            Add("NEVYPOLN[TREBOVANIE][X]", "");
            Add("NEVYPOLN[PARAMETER_ID][X]", "");

            Add("RESULTAT_PRIMECHANIE", info.Note);
            Add("ZAKLUCHENIE", "1");
            Add("SROK_DEISTV", info.ExpirationDate?.ToString("dd.MM.yyyy"));
            Add("OSOB_OTMETKI", "");
            Add("PERVICH_PROVERKA", "1");
        }
    }


}
