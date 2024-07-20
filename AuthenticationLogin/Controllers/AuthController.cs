using AuthenticationLogin.Context;
using AuthenticationLogin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using static AuthenticationLogin.Others.SHA3_256;

namespace AuthenticationLogin.Controllers;

public class AuthController(AuthDbContext context) : Controller
{
    private readonly AuthDbContext _context = context;

    public IActionResult Index(string? message = null)
    {
        return View(model: message);
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && 
            u.Password == ComputeSha3_256Hash(Encoding.UTF8.GetBytes(password)));

        if (user is null)
        {
            return RedirectToAction("Index", new { message = "Email or password is incorrect. Please try again!" });
        }

        if (user.Status is UserStatus.Blocked)
        {
            return RedirectToAction("Index", new { message = "You are a blocked user. You are not allowed to use the system" });
        }

        user.LastLoginTime = DateTime.Now;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "User", 
            new 
            {
                user.Id,
                user.Name
            });
    }

    public IActionResult ChangingPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangingPassword(string email, string newPassword, string confirmPassword)
    {
        if (ComputeSha3_256Hash(Encoding.UTF8.GetBytes(newPassword)) != 
            ComputeSha3_256Hash(Encoding.UTF8.GetBytes(confirmPassword)))
        {
            return View(model : "Passwords do not match.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user is null)
        {
            return View(model: "User not found.");
        }

        user.Password = ComputeSha3_256Hash(Encoding.UTF8.GetBytes(newPassword));
        
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Auth", new { message = "Password changed successfully!" });
    }

    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(string name, string position, string email, string password)
    {
        var tempUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (tempUser is not null)
        {
            return View(model : "User with this email already exists!");
        }

        User user = new()
        {
            Name = name,
            Email = email,
            Password = ComputeSha3_256Hash(Encoding.UTF8.GetBytes(password)),
            Position = position,
            RegistrationTime = DateTime.Now,
            Status = UserStatus.Active
        };

        await _context.AddAsync(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", new { message = $"{user.Name} successfully signed up!" });
    }

    public IActionResult Back()
    {
        return RedirectToAction("Index");
    }
}