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

namespace WebUI.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index(UserStatusEnum? filter)
        {
            if (filter == null) filter = UserStatusEnum.All;
            ViewData["UserStatusEnum"] = new SelectList(Enum.GetValues(typeof(UserStatusEnum)).Cast<UserStatusEnum>().ToList(), filter);
            switch (filter)
            {
                case UserStatusEnum.Rejected:
                    return View(await _context.User
                        .Where(users => users.IsApproved.Equals(false))
                        .Where(users => users.IsRemoved.Equals(false))
                        .ToListAsync());
                case UserStatusEnum.Accepted:
                    return View(await _context.User
                        .Where(users => users.IsApproved.Equals(true))
                        .Where(users => users.IsRemoved.Equals(false))
                        .ToListAsync());
                case UserStatusEnum.Waiting:
                    return View(await _context.User.Where(users => users.IsRemoved.Equals(false)).Where(users => users.IsApproved == null).ToListAsync());
                case UserStatusEnum.Deleted:
                    return View(await _context.User
                        .Where(users => users.IsRemoved.Equals(true))
                        .ToListAsync());
                default:
                    return View(await _context.User.ToListAsync());
            }
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
