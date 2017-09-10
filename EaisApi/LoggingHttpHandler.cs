using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EaisApi
{
    public class LoggingHttpHandler: DelegatingHandler
    {
        public LoggingHttpHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Trace.WriteLine("Request:");
            Trace.WriteLine(request.ToString());
            if (request.Content != null)
            {
                Trace.WriteLine(await request.Content.ReadAsStringAsync());
            }
            Trace.WriteLine("");

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            Trace.WriteLine("Response:");
            Trace.WriteLine(response.ToString());
            if (response.Content != null)
            {
                Trace.WriteLine(await response.Content.ReadAsStringAsync());
            }
            Trace.WriteLine("");

            return response;
        }
    }
}