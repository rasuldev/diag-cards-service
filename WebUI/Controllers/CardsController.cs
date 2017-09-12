using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EaisApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebUI.Data;
using DiagnosticCard = WebUI.Data.Entities.DiagnosticCard;
using WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebUI.Data.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;
using WebUI.Infrastructure.Pagination;
using EaisApi;
using EaisApi.Exceptions;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebUI.Infrastructure;
using WebUI.Models.CardsViewModels;

namespace WebUI.Controllers
{
    [Authorize]
    public class CardsController : Controller
    {
        private readonly AppDbContext _context;
        readonly UserManager<User> _userManager;
        bool isAdmin;
        private readonly Pager _pager;
        readonly EaistoApi _api;
        readonly IConfiguration _conf;
        private readonly ILogger<CardsController> _logger;



        public CardsController(AppDbContext context, UserManager<User> userManager, Pager pager, EaistoApi api, IConfiguration conf, ILogger<CardsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _pager = pager;
            _api = api;
            _conf = conf;
            _logger = logger;
        }

        // GET: Cards
        public async Task<IActionResult> Index(ListCardsViewModel model)
        {
            var page = _pager.CurrentPage;
            var UserId = _userManager.GetUserId(User);
            isAdmin = User.IsInRole(UserRoles.Admin);
            ViewData["isDayLimitExhausted"] = isDayLimitExhausted();

            if (model.Filter.Status == null)
                model.Filter.Status = CardStatusEnum.All;

            ViewData["CardStatusEnum"] = Misc.CreateSelectListFrom<CardStatusEnum>(model.Filter.Status);
            ViewData["SortBy"] = model.Filter.SortBy;
            ViewData["isAdmin"] = isAdmin;

            //var card = _context.DiagnosticCards.First();
            //card.UserId = UserId;
            //_context.SaveChanges();

            var cards = isAdmin
                ? _context.DiagnosticCards.Include(d => d.User)
                : _context.DiagnosticCards.Include(d => d.User).Where(item => item.UserId.Equals(UserId));

            if (isAdmin)
            {
                if (!string.IsNullOrEmpty(model.Filter.UserId))
                    cards = cards.Where(item => item.User.Id == model.Filter.UserId);
                ViewData["UsersList"] = GetUsersList("");
            }

            // Filter
            if (!string.IsNullOrEmpty(model.Filter.Fullname))
            {
                string name = "", lastname = "", patronymic = "";
                var arr = model.Filter.Fullname.Split(' ');
                if (arr.Length > 0)
                    lastname = arr[0];
                if (arr.Length > 1)
                    name = arr[1];
                if (arr.Length > 2)
                    patronymic = arr[2];
                cards = cards.Where(item => item.Lastname.StartsWith(lastname))
                .Where(item => item.Firstname.StartsWith(name))
                .Where(item => item.Patronymic.StartsWith(patronymic));
            }
            if (model.Filter.StartDate != null)
                cards = cards.Where(item => item.CreatedDate > model.Filter.StartDate);
            if (model.Filter.EndDate != null)
                cards = cards.Where(item => item.CreatedDate < model.Filter.EndDate);

            if (!string.IsNullOrEmpty(model.Filter.Regnumber))
                cards = cards.Where(item => item.RegNumber.Contains(model.Filter.Regnumber));
            if (!string.IsNullOrEmpty(model.Filter.Vin))
                cards = cards.Where(item => item.VIN.Contains(model.Filter.Vin));
            //--- Filter

            switch (model.Filter.Status)
            {
                case CardStatusEnum.Registered:
                    cards = cards.Where(c => c.RegisteredDate != null);
                    break;
                case CardStatusEnum.Unregistered:
                    cards = cards.Where(c => c.RegisteredDate == null);
                    break;
            }

            model.TotalCardsCount = await cards.CountAsync();
            model.Cards = SortList(cards, SortParamEnum.CreationDate_DESC).Skip(10 * (page - 1)).Take(10).ToList();
            return View(model);
        }

        /// <summary>
        /// Empty string means all users
        /// </summary>
        /// <param name="selectedUserId"></param>
        /// <returns></returns>
        public SelectList GetUsersList(string selectedUserId = "")
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem()
                {
                    Text = "�������� ������������",
                    Value = ""
                }
            };
            list.AddRange(_context.User.AsNoTracking().Select(user => new SelectListItem()
            {
                Text = user.UserName,
                Value = user.Id
            }));
            return new SelectList(list, "Value", "Text", selectedUserId);
        }

        private IQueryable<DiagnosticCard> SortList(IQueryable<DiagnosticCard> list, SortParamEnum sortBy)
        {
            switch (sortBy)
            {
                case SortParamEnum.RegistrationDate_ASC:
                    return list.OrderBy(s => s.RegisteredDate);
                case SortParamEnum.RegistrationDate_DESC:
                    return list.OrderByDescending(s => s.RegisteredDate);
                case SortParamEnum.User_ASC:
                    return list.OrderBy(s => s.User.Email);
                case SortParamEnum.User_DESC:
                    return list.OrderByDescending(s => s.User.Email);
                case SortParamEnum.CreationDate_DESC:
                    return list.OrderByDescending(s => s.CreatedDate);
                default:
                    return list.OrderBy(s => s.CardId);
            }
        }


        // GET: Cards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnosticCard = await _context.DiagnosticCards
                .Include(d => d.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (diagnosticCard == null)
            {
                return NotFound();
            }

            return View(diagnosticCard);
        }

        // GET: Cards/Create
        public async Task<IActionResult> Create(int? Id)
        {
            if (Id == null)
            {
                CreateOrEditInit();
                return View();
            }

            var diagnosticCard = _context.DiagnosticCards
                .Include(d => d.User)
                .SingleOrDefault(m => m.Id == Id);
            CreateOrEditInit(diagnosticCard);
            return View(diagnosticCard);
        }

        // POST: Cards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Id,CardId,Lastname,Firstname,Patronymic,VIN,IssueYear,Manufacturer,Model,BodyNumber,FrameNumber,Running,RegNumber,Weight,Category,CategoryCommon,TyreManufacturer,AllowedMaxWeight,FuelType,BrakeType,DocumentType,IsForeigner,DocumentSeries,DocumentNumber,DocumentIssueDate,DocumentIssuer,Note,ExpirationDate")] DiagnosticCard diagnosticCard)
        {
            if (ModelState.IsValid)
            {
                diagnosticCard.Id = 0;
                diagnosticCard.UserId = _userManager.GetUserId(User);
                diagnosticCard.CardId = null;
                diagnosticCard.RegisteredDate = null;
                diagnosticCard.CreatedDate = DateTime.Now;

                _context.Add(diagnosticCard);
                var f = await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            CreateOrEditInit();
            return View(diagnosticCard);
        }

        // GET: Cards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            CreateOrEditInit();
            if (id == null)
            {
                return NotFound();
            }

            var diagnosticCard = await _context.DiagnosticCards.SingleOrDefaultAsync(m => m.Id == id);
            if (diagnosticCard == null)
            {
                return NotFound();
            }
            if (diagnosticCard.RegisteredDate != null)
                return RedirectToAction("Index");
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", diagnosticCard.UserId);
            return View(diagnosticCard);
        }

        // POST: Cards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Id,CardId,Lastname,Firstname,Patronymic,VIN,IssueYear,Manufacturer,Model,BodyNumber,FrameNumber,Running,RegNumber,Weight,Category,CategoryCommon,TyreManufacturer,AllowedMaxWeight,FuelType,BrakeType,DocumentType,IsForeigner,DocumentSeries,DocumentNumber,DocumentIssueDate,DocumentIssuer,Note,ExpirationDate")] DiagnosticCard diagnosticCard)
        {
            CreateOrEditInit();
            if (id != diagnosticCard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diagnosticCard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiagnosticCardExists(diagnosticCard.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", diagnosticCard.UserId);
            return View(diagnosticCard);
        }

        // GET: Cards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnosticCard = await _context.DiagnosticCards
                .Include(d => d.User)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (diagnosticCard == null)
            {
                return NotFound();
            }
            if (diagnosticCard.RegisteredDate != null)
                return RedirectToAction("Index");


            return View(diagnosticCard);
        }

        // POST: Cards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diagnosticCard = await _context.DiagnosticCards.SingleOrDefaultAsync(m => m.Id == id);
            _context.DiagnosticCards.Remove(diagnosticCard);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Cards/Delete/5
        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string vin, string regNumber, string ticketSeries, string ticketNumber, string bodyNumber, string frameNumber)
        {
            vin = PrepareParameter(vin);
            regNumber = PrepareParameter(regNumber);
            ticketSeries = PrepareParameter(ticketSeries);
            ticketNumber = PrepareParameter(ticketNumber);
            bodyNumber = PrepareParameter(bodyNumber);
            frameNumber = PrepareParameter(frameNumber);

            // Method is not working
            string result = await _api.Search(vin, regNumber, ticketSeries, ticketNumber, bodyNumber, frameNumber);
            ViewData["Result"] = result;
            return View();
        }

        public string PrepareParameter(string par)
        {
            return par.Replace(" ", "").Length > 1 ? par : null;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            if (isDayLimitExhausted())
            {
                TempData["ErrorText"] = "����� ����������� �� ������� ��������.";
                return RedirectToAction("Index");
            }
            
            var diagnosticCard = _context.DiagnosticCards.SingleOrDefault(m => m.Id == id);
            if (diagnosticCard.UserId != _userManager.GetUserId(User))
            {
                TempData["ErrorText"] = "��� ����� ����������� ������� ������������";
                _logger.LogCritical($"{_userManager.GetUserName(User)} tried to register card with id={diagnosticCard.Id} that belongs to {diagnosticCard.User?.UserName}");
                return RedirectToAction("Index");
            }

            try
            {
                var cardId = await _api.SaveCard(diagnosticCard);
                diagnosticCard.CardId = cardId;
                diagnosticCard.RegisteredDate = DateTime.Now;
                _context.Update(diagnosticCard);
                _context.SaveChanges();
                TempData["ResultText"] = "����� � ������� " + id + " ���������� �� �����������.";
            }
            catch (CheckCardException e)
            {
                _logger.LogError(e.ToString());
                switch (e.CheckResult)
                {
                    case CheckResults.CouponAlreadyExists:
                    case CheckResults.DuplicateToday:
                        TempData["ErrorText"] =
                            "������� ��� ���� ��������� ���������� �� �� �� � ��������� VIN � ��������������� ��������������� ������";
                        break;
                    default:
                        TempData["ErrorText"] = "��� ����������� ��������� ������. ���������� ��� ��� �����.";
                        break;
                }
            }
            return RedirectToAction("Index");
        }


        private bool isDayLimitExhausted()
        {
            int limit = _conf.GetValue<int>("DayLimit");
            DateTime DateTimeNow = DateTime.Now;
            DateTime curDate = new DateTime(DateTimeNow.Year, DateTimeNow.Month, DateTimeNow.Day);

            var registeredCardsList = _context.DiagnosticCards.Where(s => s.RegisteredDate != null)
                .Where(item => item.RegisteredDate.Value.DayOfYear.Equals(curDate.DayOfYear)).ToList();
            return registeredCardsList.Count >= limit;
        }

        private bool DiagnosticCardExists(int id)
        {
            return _context.DiagnosticCards.Any(e => e.Id == id);
        }

        private void CreateOrEditInit(EaisApi.Models.DiagnosticCard diagnosticCard = null)
        {
            ViewData["CategoriesList"] = Misc.CreateSelectListFrom<VehicleCategory>(diagnosticCard?.Category);
            ViewData["CategoryCommonList"] = Misc.CreateSelectListFrom<VehicleCategoryCommon>(diagnosticCard?.CategoryCommon);
            ViewData["FuelTypesList"] = Misc.CreateSelectListFrom<FuelTypes>(diagnosticCard?.FuelType);
            ViewData["BrakeTypesList"] = Misc.CreateSelectListFrom<BrakeTypes>(diagnosticCard?.BrakeType);
            ViewData["DocumentTypesList"] = Misc.CreateSelectListFrom<DocumentTypes>(diagnosticCard?.DocumentType);
            ViewData["ManufacturesOptions"] = "";
        }
    }
}
