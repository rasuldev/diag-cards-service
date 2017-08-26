using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebUI.Data;
using WebUI.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using WebUI.Models;
using WebUI.Infrastructure.Pagination;

namespace WebUI.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Pager _pager;


        public UsersController(AppDbContext context, Pager pager)
        {
            _context = context;
            _pager = pager;
        }

        // GET: Users
        public async Task<IActionResult> Index(UserStatusEnum? filter)
        {
            var page = _pager.CurrentPage;
            if (filter == null)
            {
                if (TempData["filter"] == null)
                    filter = UserStatusEnum.All;
                else
                    filter = (UserStatusEnum)TempData["filter"];
            }

            var statusList = GetUserStatusList(filter);
            TempData["filter"] = filter;
            TempData["UserStatusEnum"] = statusList;
            TempData["UserStatusSelectedIndex"] = (int)filter;
            var resultList = new List<User>();
            switch (filter)
            {
                case UserStatusEnum.Rejected:
                    resultList = await _context.User
                        .Where(users => users.IsApproved.Equals(false))
                        .Where(users => users.IsRemoved.Equals(false))
                        .ToListAsync();
                    break;
                case UserStatusEnum.Accepted:
                    resultList = await _context.User
                        .Where(users => users.IsApproved.Equals(true))
                        .Where(users => users.IsRemoved.Equals(false))
                        .ToListAsync();
                    break;
                case UserStatusEnum.Waiting:
                    resultList = await _context.User.Where(users => users.IsRemoved.Equals(false)).Where(users => users.IsApproved == null).ToListAsync();
                    break;
                case UserStatusEnum.Deleted:
                    resultList = await _context.User
                        .Where(users => users.IsRemoved.Equals(true))
                        .ToListAsync();
                    break;
                default:
                    resultList = await _context.User.Where(user => user.IsRemoved == false).ToListAsync();
                    break;
            }
            ViewData["UsersCount"] = resultList.Count;
            return View(resultList.Skip(10 * (page - 1)).Take(10));
        }

        public SelectList GetUserStatusList(UserStatusEnum? selectedValue)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var arr = Enum.GetValues(typeof(UserStatusEnum));
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
        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _context.User
                .SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IsApproved,IsRemoved,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _context.User.SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            if (user.IsRemoved)
                return RedirectToAction("Index");
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("IsApproved,IsRemoved,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.User.SingleOrDefaultAsync(m => m.Id == id);
            user.IsRemoved = true;
            user.IsApproved = null;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Restore(string id)
        {
            if (UserExists(id))
            {
                var user = await _context.User.SingleOrDefaultAsync(m => m.Id == id);
                user.IsRemoved = false;
                _context.Update(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Accept(string id)
        {
            if (UserExists(id))
            {
                var user = await _context.User.SingleOrDefaultAsync(m => m.Id == id);
                if (!user.IsRemoved)
                {
                    user.IsApproved = true;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Reject(string id)
        {
            if (UserExists(id))
            {
                var user = await _context.User.SingleOrDefaultAsync(m => m.Id == id);
                if (!user.IsRemoved)
                {
                    user.IsApproved = false;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index");
        }


        private bool UserExists(string id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
