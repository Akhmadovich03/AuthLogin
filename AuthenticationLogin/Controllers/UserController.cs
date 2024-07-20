using AuthenticationLogin.Context;
using AuthenticationLogin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AuthenticationLogin.Controllers;

public class UserController(AuthDbContext context) : Controller
{
    private readonly AuthDbContext _context = context;

    public async Task<IActionResult> Index(User signedInUser)
    {
        List<User> users = await _context.Users.ToListAsync();
        
        return View((users, signedInUser));
    }

    public IActionResult Logout()
    {
        return RedirectToAction("Index", "Auth");
    }

    [HttpPost]
    public async Task<IActionResult> Block(string userIds, User user)
    {
        var ids = JsonConvert.DeserializeObject<int[]>(userIds);

        if (ids is null)
        {
            return RedirectToAction("Index",
                new
                {
                    user.Id,
                    user.Name
                });
        }

        bool isBlocked = false;

        foreach (var id in ids)
        {
            var tempUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            
            if (tempUser is null)
            {
                continue;
            }

            if (tempUser.Id == user.Id)
            {
                isBlocked = true;
            }

            tempUser.Status = UserStatus.Blocked;
        }
        await _context.SaveChangesAsync();
        
        if (!isBlocked)
        {
            return RedirectToAction("Index", 
                new
                {
                    user.Id,
                    user.Name
                });
        }

        return RedirectToAction("Index", "Auth", new { message = "You are a blocked user. You are not allowed to use the system" });
    }

    [HttpPost]
    public async Task<IActionResult> Unlock(string userIds, User user)
    {
        var ids = JsonConvert.DeserializeObject<int[]>(userIds);

        if (ids is null)
        {
            return RedirectToAction("Index",
                new
                {
                    user.Id,
                    user.Name
                });
        }

        foreach (var id in ids)
        {
            var tempUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            
            if (tempUser != null)
            {
                tempUser.Status = UserStatus.Active;
            }
        }
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index", 
            new
            {
                user.Id,
                user.Name
            });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string userIds, User user)
    {
        var ids = JsonConvert.DeserializeObject<int[]>(userIds);
        
        if (ids is null)
        {
            return RedirectToAction("Index",
                new
                {
                    user.Id,
                    user.Name
                });
        }

        bool isDeleted = false;
        
        foreach (var id in ids)
        {
            var tempUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            
            if (tempUser is null)
            {
                continue;
            }

            if (tempUser.Id == user.Id)
            {
                isDeleted = true;
            }

            _context.Users.Remove(tempUser);
        }
        
        await _context.SaveChangesAsync();
        
        if (!isDeleted)
        {
            return RedirectToAction("Index", 
                new
                {
                    user.Id,
                    user.Name
                });
        }

        return RedirectToAction("Index", "Auth");
    }
}
