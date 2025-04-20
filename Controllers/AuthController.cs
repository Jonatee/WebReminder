using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebReminder.Context;
using WebReminder.Entities;
using WebReminder.Models.DTOs;
using WebReminder.Services.Interfaces;

namespace WebReminder.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserContext _context;


        public AuthController(IUserService userService,IUserContext userContext)
        {
            _userService = userService;
            _context = userContext;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRequestModel model)
        {
            var registerUser = await _userService.RegisterUser(model);
            if (registerUser == null)
                return View(model);
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestModel model)
        {
            var login = await _userService.LoginUser(model);
            if (login == null)
            {
                ModelState.AddModelError("Password", "Invalid Password");
                return View(model);
            }
            return RedirectToAction("AllReminders","Reminder");
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

    }
}
