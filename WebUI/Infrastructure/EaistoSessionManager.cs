using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebUI.Data;
using WebUI.Data.Entities;

namespace WebUI.Infrastructure
{
    public class EaistoSessionManager
    {
        public const string EaistoSessionCookieName = "EaistoSessionId";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        private string _newSessionId;

        public EaistoSessionManager(IHttpContextAccessor httpContextAccessor, AppDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public string GetSessionId() => _httpContextAccessor.HttpContext?.Request?.Cookies[EaistoSessionCookieName] ?? _newSessionId;

        public async Task<string> StartNewSession()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var eaistoSession = new EaistoSession();
            _context.EaistoSessions.Add(eaistoSession);
            await _context.SaveChangesAsync();
            httpContext.Response.Cookies.Append(EaistoSessionCookieName, eaistoSession.Id.ToString());
            _newSessionId = eaistoSession.Id;
            return _newSessionId;
        }


    }
}
