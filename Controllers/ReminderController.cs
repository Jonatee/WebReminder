using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebReminder.Context;
using WebReminder.Entities;
using WebReminder.Models.DTOs;
using WebReminder.Services.Interfaces;

namespace WebReminder.Controllers
{
    [Authorize]
    public class ReminderController : Controller
    {
        private readonly ReminderDb _context;
        private readonly IReminderService _service;

        public ReminderController(ReminderDb context, IReminderService reminderService)
        {
            _context = context;
            _service = reminderService;
        }

        // GET: Reminder
        public async Task<IActionResult> AllReminders()
        {
            var reminders = await _service.GetAllReminders();
            if (!reminders.Success)
            { 
                TempData["InfoMessage"] = reminders.Message;
                return View();
            }
            TempData["InfoMessage"] = reminders.Message;
            return View(reminders);
        }
        public async Task<IActionResult> Trash()
        {
            var reminders = await _service.GetAllDeletedReminders();
            if (reminders is null)
            {
                TempData["InfoMessage"] = "Trash is Empty";
                return RedirectToAction("AllReminders");
            }
            TempData["InfoMessage"] = "These are your Deleted Reminders";
            return View(reminders);
        }
        [HttpPost]
        public async Task<IActionResult> DeletePermanently(Guid id)
        {
            var reminders = await _service.PermanentDeleteReminder(id);
            if (!reminders.Success)
            {
                TempData["InfoMessage"] = reminders.Message;
                return RedirectToAction("Trash");
            }
            TempData["SuccessMessage"] = reminders.Message;
            return RedirectToAction("Trash");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteReminder(Guid id)
        {
            var reminders = await _service.DeleteReminder(id);
            if (!reminders.Success)
            {
                TempData["InfoMessage"] = reminders.Message;
                return RedirectToAction("Trash");
            }
            TempData["SuccessMessage"] = reminders.Message;
            return RedirectToAction("Trash");
        }
        [HttpPost]
        public async Task<IActionResult> RestoreReminder(Guid id)
        {
            var reminders = await _service.RestoreDeletedReminder(id);
            if (!reminders.Success)
            {
                TempData["InfoMessage"] = reminders.Message;
                return RedirectToAction("Trash");
            }
            TempData["SuccessMessage"] = reminders.Message;
            return View("AllReminders");
        }
        public async Task<IActionResult> SentReminders()
        {
            var reminders = await _service.GetSentReminders();
            if (reminders is null)
            {
                TempData["InfoMessage"] = "You Have No Sent Reminders";
                return RedirectToAction("AllReminders");
            }
            TempData["SuccessMessage"] = "All your Sent Reminders";
            return View(reminders);
        }

        // GET: Reminder/Details/5
        public async Task<IActionResult> ViewReminder(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var reminder = await _service.GetReminder(id);
            if (!reminder.Success)
            {
                ViewBag.Message = "Reminder Not Accessible";
                TempData["InfoMessage"] = reminder.Message;
                return View("AllReminders");
            }
            TempData["SuccessMessage"] = reminder.Message;
            return View(reminder.Data);
        }

        // GET: Reminder/Create
        public IActionResult CreateReminder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReminder(ReminderRequestModel reminder)
        {
            var result = await _service.CreateReminder(reminder);
            if (!result.Success)
            {
                TempData["WarningMessage"] = result.Message;
                return View(reminder);
            }
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("AllReminders");
        }
        

        public async Task<IActionResult> EditReminder(Guid id)
        {
            var result = await _service.GetReminder(id);
            if (!result.Success)
            {
                TempData["InfoMessage"] = result.Message;
                return View();
            }
            var viewModel = new ReminderUpdateModel
            {
                Description = result.Data.Description,
                DueDate = result.Data.DueDate,
                ReminderId = result.Data.Id,
                Title = result.Data.Title,
            };
            ViewBag.CurrentImage = result.Data.ImageUrl;
            TempData["InfoMessage"] = "Edit your Reminder";
            return View(viewModel);
        }

        
        [HttpPost]
        public async Task<IActionResult> EditReminder(ReminderUpdateModel reminder)
        {
            var result = await _service.UpdateReminder(reminder);
            if (!result.Success)
            {
                TempData["InfoMessage"] = result.Message;
                return View(reminder);
            }
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("AllReminders");
        }
    }
}

      
