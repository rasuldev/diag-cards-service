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
using Newtonsoft.Json;

namespace WebUI.Controllers
{
    [Authorize]
    public class CardsController : Controller
    {
        private readonly AppDbContext _context;
        UserManager<User> _userManager;
        bool isAdmin;
        private readonly Pager _pager;
        EaistoApi _api;


        public CardsController(AppDbContext context, UserManager<User> userManager, Pager pager, EaistoApi api)
        {
            _context = context;
            _userManager = userManager;
            _pager = pager;
            _api = api;
        }

        // GET: Cards
        public async Task<IActionResult> Index(SortParamEnum sortBy,
            string Regnumber, string VIN, string FIO, DateTime? StartDate, DateTime? EndDate, CardStatusEnum? filter)
        {
            var page = _pager.CurrentPage;
            var UserId = _userManager.GetUserId(User);
            isAdmin = User.IsInRole(UserRoles.Admin);

            if (filter == null)
            {
                if (TempData["FilterCard"] == null)
                    filter = CardStatusEnum.Registered;
                else
                    filter = (CardStatusEnum)TempData["FilterCard"];
            }

            TempData["FilterCard"] = filter;
            TempData["CardStatusEnum"] = GetCardStatusList(filter);
            TempData["CardStatusSelectedIndex"] = (int)filter;
            TempData["SortBy"] = sortBy;
            TempData["isAdmin"] = isAdmin;

            var appDbContext = isAdmin
                ? _context.DiagnosticCards.Include(d => d.User).Where(item => item.UserId != null)
                : _context.DiagnosticCards.Include(d => d.User).Where(item => item.UserId.Equals(UserId));

            // Filter
            if (FIO != null && FIO != "")
            {
                string name = "", lastname = "", patronymic = "";
                var arr = FIO.Split(' ');
                if (arr.Length > 0)
                    lastname = arr[0];
                if (arr.Length > 1)
                    name = arr[1];
                if (arr.Length > 2)
                    patronymic = arr[2];
                appDbContext = appDbContext.Where(item => item.Lastname.StartsWith(lastname))
                .Where(item => item.Firstname.StartsWith(name))
                .Where(item => item.Patronymic.StartsWith(patronymic));
            }
            if (StartDate != null)
                appDbContext = appDbContext.Where(item => item.CreatedDate > StartDate);
            if (EndDate != null)
                appDbContext = appDbContext.Where(item => item.CreatedDate < EndDate);

            if (Regnumber != null && Regnumber != "")
                appDbContext = appDbContext.Where(item => item.RegNumber.Contains(Regnumber));
            if (VIN != null && VIN != "")
                appDbContext = appDbContext.Where(item => item.VIN.Contains(VIN));
            //--- Filter

            // Split to 2 list registered and not registered items
            var registeredCardsList = appDbContext.Where(s => s.RegisteredDate != null).ToList();
            var notRegisteredCardsList = appDbContext.Where(s => s.RegisteredDate == null).ToList();
            //--- Split to 2 list registered and not registered items

            ViewData["RegCardsCount"] = registeredCardsList.Count;
            ViewData["NotRegCardsCount"] = notRegisteredCardsList.Count;

            UserCardsBox box = new UserCardsBox();
            switch (filter)
            {
                case CardStatusEnum.Registered:
                    box.RegisteredCards = SortList(registeredCardsList, sortBy).Skip(10 * (page - 1)).Take(10).ToList();
                    break;
                case CardStatusEnum.Unregistered:
                    box.NotRegisteredCards = SortList(notRegisteredCardsList, SortParamEnum.CreationDate_DESC).Skip(10 * (page - 1)).Take(10).ToList();
                    break;
            }
            return View(box);
        }

        public SelectList GetCardStatusList(CardStatusEnum? selectedValue)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var arr = Enum.GetValues(typeof(CardStatusEnum));
            for (int i = 0; i < arr.Length; i++)
            {
                list.Add(new SelectListItem()
                {
                    Text = arr.GetValue(i).ToString(),
                    Value = i.ToString(),
                    Selected = arr.GetValue(i).ToString().Equals(selectedValue.ToString())
                });
            }
            return new SelectList(list, "Value", "Text");
        }

        public List<DiagnosticCard> SortList(List<DiagnosticCard> list, SortParamEnum sortBy)
        {
            var resultList = new List<DiagnosticCard>();
            switch (sortBy)
            {
                case SortParamEnum.RegistrationDate_ASC:
                    resultList = list.OrderBy(s => s.RegisteredDate).ToList();
                    break;
                case SortParamEnum.RegistrationDate_DESC:
                    resultList = list.OrderByDescending(s => s.RegisteredDate).ToList();
                    break;
                case SortParamEnum.User_ASC:
                    if (isAdmin)
                        resultList = list.OrderBy(s => s.User.Email).ToList();
                    break;
                case SortParamEnum.User_DESC:
                    if (isAdmin)
                        resultList = list.OrderByDescending(s => s.User.Email).ToList();
                    break;
                case SortParamEnum.CreationDate_DESC:
                    resultList = list.OrderByDescending(s => s.CreatedDate).ToList();
                    break;
                default:
                    resultList = list.OrderBy(s => s.CardId).ToList();
                    break;
            }
            return resultList;
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            CreateOrEditInit();
            if (Id != null)
            {
                var diagnosticCard = _context.DiagnosticCards
                .Include(d => d.User)
                .SingleOrDefault(m => m.Id == Id);
                if (diagnosticCard != null && diagnosticCard.RegisteredDate != null)
                {
                    diagnosticCard.CreatedDate = DateTime.Now;
                    return View(diagnosticCard);
                }
                else
                    return RedirectToAction("Index", "Cards");
            }
            return View();
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", diagnosticCard.UserId);
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

        private bool DiagnosticCardExists(int id)
        {
            return _context.DiagnosticCards.Any(e => e.Id == id);
        }

        private async void CreateOrEditInit()
        {
            ViewData["CategoriesList"] = new SelectList(InitVehicleCategoryList(), "code", "value");
            ViewData["CategoryCommonList"] = new SelectList(InitVehicleCategoryCommonList(), "code", "value");
            ViewData["FuelTypesList"] = new SelectList(InitFuelTypesList(), "code", "value");
            ViewData["BrakeTypesList"] = new SelectList(InitBrakeTypesList(), "code", "value");
            ViewData["DocumentTypesList"] = new SelectList(InitDocumentTypesList(), "code", "value");
            ViewData["ManufacturesOptions"] = await InitManufacturesListForHTML();
        }

        private List<object> InitVehicleCategoryList()
        {
            var categories = new List<object>() { new { code = "", value = "" } };
            categories.AddRange(
                Enum.GetValues(typeof(VehicleCategory)).Cast<VehicleCategory>()
                    .Select(x => new { code = x, value = x.ToString() }));
            return categories;
        }
        private List<object> InitVehicleCategoryCommonList()
        {
            var categories = new List<object>() { new { code = "", value = "" } };
            categories.AddRange(
                Enum.GetValues(typeof(VehicleCategoryCommon)).Cast<VehicleCategoryCommon>()
                    .Select(x => new { code = x, value = x.ToString() }));
            return categories;
        }
        private List<object> InitFuelTypesList()
        {
            var categories = new List<object>() { new { code = "", value = "" } };
            categories.AddRange(
                Enum.GetValues(typeof(FuelTypes)).Cast<FuelTypes>()
                    .Select(x => new { code = x, value = x.ToString() }));
            return categories;
        }
        private List<object> InitBrakeTypesList()
        {
            var categories = new List<object>() { new { code = "", value = "" } };
            categories.AddRange(
                Enum.GetValues(typeof(BrakeTypes)).Cast<BrakeTypes>()
                    .Select(x => new { code = x, value = x.ToString() }));
            return categories;
        }
        private List<object> InitDocumentTypesList()
        {
            var categories = new List<object>() { new { code = "", value = "" } };
            categories.AddRange(
                Enum.GetValues(typeof(DocumentTypes)).Cast<DocumentTypes>()
                    .Select(x => new { code = x, value = x.ToString() }));
            return categories;
        }
        public async Task<String> InitManufacturesListForHTML()
        {
            var result = await _api.GetAllManufacturers();
            ManufacturerSuggestions sug = JsonConvert.DeserializeObject<ManufacturerSuggestions>(result);
            string options = "";
            foreach (var item in sug.Suggestions)
            {
                options += "<option>" + item + "</option>\r\n";
            }
            return options;
        }
    }
}
