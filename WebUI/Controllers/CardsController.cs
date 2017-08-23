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

namespace WebUI.Controllers
{
    public class CardsController : Controller
    {
        private readonly AppDbContext _context;

        public CardsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Cards
        public async Task<IActionResult> Index(string crSortOrder, string regSortOrder)
        {
            ViewData["CreateDateSortParm"] = String.IsNullOrEmpty(crSortOrder) ? "desc" : "asc";
            ViewData["RegDateSortParm"] = String.IsNullOrEmpty(regSortOrder) ? "desc" : "asc";

            var appDbContext = _context.DiagnosticCards.Include(d => d.User);
            var registeredCardsList = appDbContext.Where(s => s.RegisteredDate != null);
            var notRegisteredCardsList = appDbContext.Where(s => s.RegisteredDate == null);

            switch (regSortOrder)
            {
                case "desc":
                    registeredCardsList = registeredCardsList.OrderByDescending(s => s.RegisteredDate);
                    break;
                case "asc":
                    registeredCardsList = registeredCardsList.OrderBy(s => s.RegisteredDate);
                    break;
                default:
                    registeredCardsList = registeredCardsList.OrderBy(s => s.RegisteredDate);
                    break;
            }

            switch(crSortOrder)
            {
                case "desc":
                    notRegisteredCardsList = notRegisteredCardsList.OrderByDescending(s => s.CreatedDate);
                    break;
                case "asc":
                    notRegisteredCardsList = notRegisteredCardsList.OrderBy(s => s.CreatedDate);
                    break;
                default:
                    notRegisteredCardsList = notRegisteredCardsList.OrderBy(s => s.CreatedDate);
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
