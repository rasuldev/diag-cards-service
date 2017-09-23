using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using EaisApi;
using Microsoft.AspNetCore.Mvc;
using WebUI.Infrastructure.Pagination;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
    	private readonly EaistoApi _api;
        private readonly Pager _pager;

        public HomeController(EaistoApi api, Pager pager)
        {
            _api = api;
            _pager = pager;
        }

        public async Task<Stream> Init()
        {
            return await _api.InitRemoteSession();
        }

	

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
