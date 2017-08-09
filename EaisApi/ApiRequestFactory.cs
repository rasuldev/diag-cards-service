using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EaisApi
{
    public class ApiRequestFactory
    {
        private const string BaseUrl = "http://eaisto.gibdd.ru/ru/arm/";
        private const string SessionKey = "PHPSESSID";
        private string SessionId { get; set; }

        public ApiRequestFactory(string sessionId)
        {
            SessionId = sessionId;
        }

        /// <summary>
        /// Creates request for login page
        /// </summary>
        /// <returns></returns>
        public HttpRequestMessage InitSignIn()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl);
            return request;
        }



        private void SetSessionCookie(HttpRequestMessage request)
        {
            request.Headers.Add("Cookie", $"{SessionKey}={SessionId}");
        }


    }
}
