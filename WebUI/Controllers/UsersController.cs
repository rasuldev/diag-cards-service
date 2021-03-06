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
using Microsoft.AspNetCore.Identity;
using WebUI.Infrastructure;
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
                filter = UserStatusEnum.All;

            //var statusList = GetUserStatusList(filter);
            ViewData["UserStatusEnum"] = Misc.CreateSelectListFrom<UserStatusEnum>(filter);
            IQueryable<User> resultList;
            switch (filter)
            {
                case UserStatusEnum.Rejected:
                    resultList = _context.User
                        .Where(users => users.IsApproved.Equals(false))
                        .Where(users => users.IsRemoved.Equals(false));
                    break;
                case UserStatusEnum.Accepted:
                    resultList = _context.User
                        .Where(users => users.IsApproved.Equals(true))
                        .Where(users => users.IsRemoved.Equals(false));
                    break;
                case UserStatusEnum.Waiting:
                    resultList = _context.User
                        .Where(users => users.IsRemoved.Equals(false))
                        .Where(users => users.IsApproved == null);
                    break;
                case UserStatusEnum.Deleted:
                    resultList = _context.User
                        .Where(users => users.IsRemoved.Equals(true));
                    break;
                default:
                    resultList = _context.User.Where(user => user.IsRemoved == false);
                    break;
            }
            ViewData["UsersCount"] = await resultList.CountAsync();
            return View(await resultList.GetPage(page, 10).ToListAsync());
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

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound("������������ �� ������");
            }
            return View(new ResetUserPassModel() { UserId = user.Id, Username = user.UserName, Password = "123" });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetUserPassModel userModel, [FromServices] UserManager<User> userManager)
        {
            var user = await _context.User.FindAsync(userModel.UserId);
            if (user == null)
            {
                return NotFound("������������ �� ������");
            }
            await userManager.RemovePasswordAsync(user);
            await userManager.AddPasswordAsync(user, userModel.Password);
            this.AddInfoMessage("������ ������� �������!");
            return RedirectToAction("Index");
        }

        private bool UserExists(string id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
