using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUI.Data;
using WebUI.Models;
using WebUI.Models.ReportViewModels;

namespace WebUI.Controllers
{
    [Authorize(Roles = UserRoles.AdminAndSpectator)]
    public class ReportController : Controller
    {
        private readonly AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ReportFilter(){ End = DateTime.UtcNow.AddHours(3)});
        }

        [HttpPost]
        public async Task<IActionResult> Index(ReportFilter filter)
        {
            if (ModelState.IsValid)
            {
                var cards = _context.DiagnosticCards.Where(c => c.IsRegistered);
                if (filter.Start != null)
                    cards = cards.Where(item => item.CreatedDate >= filter.Start);
                if (filter.End == null)
                    filter.End = DateTime.UtcNow.AddHours(3);
                if (filter.End != null)
                {
                    cards = cards.Where(item => item.CreatedDate < filter.End.Value.AddDays(1));
                }

                var stat = await cards.Join(_context.User, card => card.UserId, user => user.Id,
                        (c,u)=> new {userName = u.UserName, c.Id})
                    .GroupBy(c => c.userName, (userName, userCards) => new {userName, count = userCards.Count()})
                    .AsNoTracking()
                    .ToListAsync();

                var sb = new StringBuilder();

                sb.AppendLine(
                    $"Отчет за период: {filter.Start?.ToString("dd.MM.yyyy") ?? "_"} - {filter.End?.ToString("dd.MM.yyyy")}\r\n");
                sb.AppendLine("Пользователь;Количество карт");
                foreach (var item in stat)
                {
                    sb.AppendLine($"{item.userName};{item.count}");
                }

                //var stream = await generator.Generate(card, stamp);
                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv",
                    $"Отчет за {filter.Start?.ToString("dd.MM.yyyy") ?? "_"} - {filter.End?.ToString("dd.MM.yyyy")}.csv");
            }
                //return RedirectToAction("Index");
            return View(filter);
        }
    }
}