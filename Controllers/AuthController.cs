﻿using System;
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
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using WebReminder.ExternalServices.Interfaces;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace WebReminder.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserContext _context;
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailService;


        public AuthController(IUserService userService,IUserContext userContext,IMemoryCache cache,IEmailService emailService)

        {
            _userService = userService;
            _context = userContext;
            _cache = cache;
            _emailService = emailService;

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
            {
                TempData["ErrorMessage"] = "User Already Exists. Please Login";
                return View(model);
            }
            var recoverycode = new Random().Next(1000, 10000);
            var email = model.Email;
            var cacheSet = _cache.Set<string>(email, recoverycode.ToString(), TimeSpan.FromMinutes(10));
            var emailSender = new EmailConfirmationRequestModel
            {
                VerificationCode = recoverycode.ToString(),
                 To = email,
            };
            var confirmation =  await _emailService.SendEmailConfirmation(emailSender);
            if (!confirmation)
            {
                ModelState.AddModelError("Email", "Email not Sent");
                TempData["WarningMessage"] = "Email Not Sent,Please Try Again";
                return View();
            }
            var verifyemail = new VerifyEmail
            {
                Email = email
            };
            TempData["InfoMessage"] = "Code has been sent to yor mail for verification";
            return View("VerifyEmail",verifyemail);
        }

        public IActionResult VerifyEmail()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmail verifyEmail)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var userAccountCheck =  await _userService.GetUser(verifyEmail.Email);
            if(userAccountCheck == null)
            {
                TempData["WarningMessage"] = "Please Register Before using this Service";
                ModelState.AddModelError("Email", "No Account with the email found");
                return View(verifyEmail);
            }
            _cache.TryGetValue(verifyEmail.Email, out string? storedCache);
            if(storedCache == verifyEmail.Code)
            {
              var user =  await _userService.UpdateUser(verifyEmail.Email);
                if (user != null)
                {
                    TempData["SuccessMessage"] = "Successfully Verified";
                    return RedirectToAction("Login");
                }
                TempData["ErrorMessage"] = "Could not verify You ,Please Tray Again or Later";
                return View();
            }
            ModelState.AddModelError("Code", "Invalid Code");
            return View(verifyEmail);
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
                TempData["ErrorMessage"] = "Incorrect Credentials";
                ModelState.AddModelError("Password", "Invalid Credentials");
                return View(model);
            }
            if (!login.IsVerified)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var recoverycode = new Random().Next(1000, 10000);
                var email = model.Email;
                var cacheSet = _cache.Set<string>(email, recoverycode.ToString(), TimeSpan.FromMinutes(10));
                var emailSender = new EmailConfirmationRequestModel
                {
                    VerificationCode = recoverycode.ToString(),
                    To = email,
                };
                var confirmation = await _emailService.SendEmailConfirmation(emailSender);
                if (!confirmation)
                {
                    TempData["ErrorMessage"] = "Email Not Sent Try Again Later";
                    ModelState.AddModelError("Email", "Email not Sent");
                    return View();
                }
                return View("VerifyEmail");
            }
            TempData["SuccessMessage"] = "Login Successful";
            return RedirectToAction("AllReminders","Reminder");
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "Logged Out Successfully";
            return RedirectToAction("Login");
        }

    }
}
