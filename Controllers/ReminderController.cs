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
                ViewBag.Message = reminders.Message;
                return View();
            }
            return View(reminders);
        }
        public async Task<IActionResult> Trash()
        {
            var reminders = await _service.GetAllDeletedReminders();
            if (reminders is null)
            {
                ViewBag.Message = "No Deleted Reminders";
                return RedirectToAction("AllReminders");
            }
            return View(reminders);
        }
        [HttpPost]
        public async Task<IActionResult> DeletePermanently(Guid id)
        {
            var reminders = await _service.PermanentDeleteReminder(id);
            if (!reminders.Success)
            {
                ViewBag.Message = reminders.Message;
                return RedirectToAction("Trash");
            }
            return RedirectToAction("AllReminders");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteReminder(Guid id)
        {
            var reminders = await _service.DeleteReminder(id);
            if (!reminders.Success)
            {
                ViewBag.Message = reminders.Message;
                return RedirectToAction("Trash");
            }
            return RedirectToAction("ViewReminders", new { id } );
        }
        [HttpPost]
        public async Task<IActionResult> RestoreReminder(Guid id)
        {
            var reminders = await _service.RestoreDeletedReminder(id);
            if (!reminders.Success)
            {
                ViewBag.Message = reminders.Message;
                return RedirectToAction("AllReminders");
            }
            return View("Trash");
        }
        public async Task<IActionResult> SentReminders()
        {
            var reminders = await _service.GetSentReminders();
            if (reminders is null)
            {
                ViewBag.Message = "No Sent Reminders";
                return RedirectToAction("AllReminders");
            }
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
            if (reminder == null)
            {
                ViewBag.Message = "Reminder Not Accessible";
                return View("AllReminders");
            }

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
                return View(reminder);

            return RedirectToAction("AllReminders");
        }
        

        public IActionResult EditReminder()
        {
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> EditReminder(ReminderUpdateModel reminder)
        {
            var result = await _service.UpdateReminder(reminder);
            if (!result.Success)
                return View(reminder);

            return RedirectToAction("AllReminders");
        }
    }
}

      
