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

namespace WebUI.Controllers
{
    [Authorize]
    public class CardsController : Controller
    {
        private readonly AppDbContext _context;
        UserManager<User> _userManager;

        public CardsController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Cards
        public async Task<IActionResult> Index(SortParamEnum sortParamTable2,
            string Regnumber, string VIN, string FIO, DateTime StartDate, DateTime EndDate)
        {
            var UserId = _userManager.GetUserId(User);
            //var UserType = this.User.Identity.AuthenticationType;
            var isAdmin = User.IsInRole("admin");
            ViewData["SortParamTable2"] = sortParamTable2;
            ViewData["isAdmin"] = isAdmin;

            //if (isAdmin)
            //    ViewData["UserDateSortParameter"] = String.IsNullOrEmpty(userSortOrder) ? "desc" : "asc";

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
            if (StartDate != null && EndDate != null)
            {
                if (EndDate == new DateTime(1, 1, 1))
                    EndDate = DateTime.Now;
                appDbContext = appDbContext.Where(item => item.CreatedDate > StartDate && item.CreatedDate < EndDate);
            }
            if (Regnumber != null && Regnumber != "")
                appDbContext = appDbContext.Where(item => item.RegNumber.Contains(Regnumber));
            if (VIN != null && VIN != "")
                appDbContext = appDbContext.Where(item => item.VIN.Contains(VIN));
            //--- Filter

            var registeredCardsList = appDbContext.Where(s => s.RegisteredDate != null);
            var notRegisteredCardsList = appDbContext.Where(s => s.RegisteredDate == null);

            switch (sortParamTable2)
            {
                case SortParamEnum.RegistrationDate_ASC:
                    registeredCardsList = registeredCardsList.OrderBy(s => s.RegisteredDate);
                    break;
                case SortParamEnum.RegistrationDate_DESC:
                    registeredCardsList = registeredCardsList.OrderByDescending(s => s.RegisteredDate);
                    break;
                case SortParamEnum.User_ASC:
                    if (isAdmin)
                        registeredCardsList = registeredCardsList.OrderBy(s => s.User);
                    break;
                case SortParamEnum.User_DESC:
                    if (isAdmin)
                        registeredCardsList = registeredCardsList.OrderByDescending(s => s.User);
                    break;
                default:
                    registeredCardsList = registeredCardsList.OrderBy(s => s.CardId);
                    break;
            }


            UserCardsBox box = new UserCardsBox();
            box.RegisteredCards = registeredCardsList.ToList();
            box.NotRegisteredCards = notRegisteredCardsList.ToList();
            return View(box);
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
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            CreateOrEditInit();
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
                diagnosticCard.RegisteredDate = DateTime.Now;
                _context.Add(diagnosticCard);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", diagnosticCard.UserId);
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

        private void CreateOrEditInit()
        {
            ViewData["CategoriesList"] = new SelectList(InitVehicleCategoryList(), "code", "value");
            ViewData["CategoryCommonList"] = new SelectList(InitVehicleCategoryCommonList(), "code", "value");
            ViewData["FuelTypesList"] = new SelectList(InitFuelTypesList(), "code", "value");
            ViewData["BrakeTypesList"] = new SelectList(InitBrakeTypesList(), "code", "value");
            ViewData["DocumentTypesList"] = new SelectList(InitDocumentTypesList(), "code", "value");
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
    }
}
