using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EaisApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        private EaistoApi _api;

        public HomeController(EaistoApi api)
        {
            _api = api;
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
            ViewData["Message"] = "Your application description page.";
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
