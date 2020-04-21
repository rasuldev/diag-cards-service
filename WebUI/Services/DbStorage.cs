using EaisApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebUI.Data;
using WebUI.Data.Entities;
using WebUI.Infrastructure;

namespace WebUI.Services
{
    public class DbStorage : IUserStorage
    {
        private readonly EaistoSessionManager _eaistoSessionManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<DbStorage> _logger;

        public DbStorage(EaistoSessionManager eaistoSessionManager, IHttpContextAccessor httpContextAccessor, 
            AppDbContext context, UserManager<User> userManager, ILogger<DbStorage> logger)
        {
            _eaistoSessionManager = eaistoSessionManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _userManager = userManager;
            this._logger = logger;
        }

        public async Task<ApiUserData> LoadData()
        {
            var sessionId = _eaistoSessionManager.GetSessionId();
            if (sessionId == null)
            {
                _logger.LogInformation("Eaisto session id is null");
                return null;
            }
            var sessionData = await _context.EaistoSessions.SingleOrDefaultAsync(s => s.Id == sessionId);
            if (sessionData == null)
                return null;
            return new ApiUserData(sessionData.Cookies, sessionData.CaptchaId);
        }

        public async Task SaveData(ApiUserData data)
        {
            var sessionId = _eaistoSessionManager.GetSessionId();
            if (sessionId == null)
                sessionId = await _eaistoSessionManager.StartNewSession();
            var sessionData = await _context.EaistoSessions.SingleOrDefaultAsync(s => s.Id == sessionId);
            if (sessionData == null)
                return;
            sessionData.CaptchaId = data.CaptchaId;
            sessionData.Cookies = data.Cookies;
            await _context.SaveChangesAsync();
        }
    }
}
