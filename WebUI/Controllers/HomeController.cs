using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EaisApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebUI.Infrastructure.Pagination;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        private EaistoApi _api;
        private readonly Pager _pager;

        public HomeController(EaistoApi api, Pager pager)
        {
            _api = api;
            _pager = pager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<Stream> Init()
        {
            return await _api.InitRemoteSession();
        }

        public IActionResult About()
        {
            var page = _pager.CurrentPage;
            ViewData["Message"] = $"Page {page}.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
