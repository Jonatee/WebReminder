using Supabase.Gotrue;
using System.Security.Claims;
using WebReminder.Context;
using WebReminder.Entities;
using WebReminder.ExternalServices.Implementations;
using WebReminder.ExternalServices.Interfaces;
using WebReminder.Models.DTOs;
using WebReminder.Repositories.Implementaions;
using WebReminder.Repositories.Interfaces;
using WebReminder.Services.Interfaces;

namespace WebReminder.Services.Implementaions
{
    public class ReminderService : IReminderService
    {
        private readonly IReminderRespository _reminderRepository;
        private readonly IUserContext _context;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IFileService _fileService;
        public ReminderService(IReminderRespository reminderRespository,IHttpContextAccessor httpContextAccessor, IUserContext context,IEmailService emailService,IFileService fileService)
        {
            _reminderRepository = reminderRespository;
            _fileService = fileService;
            _contextAccessor = httpContextAccessor;
            _context = context;
            _emailService = emailService;
        }
        public async Task<IEnumerable<ReminderResponseModel>> BulkCreate(List<ReminderRequestModel> reminderRequests)
        {
            if (reminderRequests == null || !reminderRequests.Any())
                return Enumerable.Empty<ReminderResponseModel>();
            var userId = _context.UserId;

            var reminders = reminderRequests
                .Where(r => !string.IsNullOrWhiteSpace(r.Title))
                .Select(r => new Reminder
                {
                    DueDate = r.DueDate,
                    Title = r.Title,
                    Description = r.Description,
                    UserId = userId,
                    LastModified = DateTime.UtcNow
                })
                .ToList();

            if (!reminders.Any())
                return Enumerable.Empty<ReminderResponseModel>();

            await _reminderRepository.AddRangeAsync(reminders);
            await _reminderRepository.SaveChanges();

            var response = reminders.Select(r => new ReminderResponseModel
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                DueDate = r.DueDate
            });

            return response;
        }


        public async Task<BaseResponse<ReminderResponseModel>> CreateReminder(ReminderRequestModel request)
        {
            var checkIfReminderExists = await _reminderRepository.GetReminderAsync(request.Title, request.Description,request.DueDate);
            if(checkIfReminderExists is not null)
            {
                return new BaseResponse<ReminderResponseModel>
                {
                    Message = "Reminder ALl Exists",
                    Success = false
                };
            }
            if (!string.IsNullOrEmpty(request.Title) && request.DueDate > DateTime.Now && !string.IsNullOrEmpty(request.Description))
            {
                var newReminder = new Reminder
                {
                    Title = request.Title,
                    DueDate = request.DueDate,
                    ImageUrl = request.Image != null ? await _fileService.UploadImage(request.Image) :string.Empty,
                    Description = request.Description,
                    UserId = _context.UserId,
                    LastModified = DateTime.Now
                };
                var createReminder = await _reminderRepository.AddReminderAsync(newReminder);
                if (createReminder is null) 
                {
                    return new BaseResponse<ReminderResponseModel>
                    {
                        Message = "Cannot Create Reminder",
                        Success = false
                    };
                }
                return new BaseResponse<ReminderResponseModel>
                {
                    Message = "Reminder Created Successfully",
                    Success = true,
                    Data = new ReminderResponseModel
                    {
                        DateCreated = createReminder.DateCreated,
                        Description = createReminder.Description,
                        DueDate = createReminder.DueDate,
                        Id = createReminder.Id,
                        ImageUrl = createReminder.ImageUrl,
                        IsDeleted = createReminder.IsDeleted,
                        IsSent = createReminder.IsSent,
                        Title = createReminder.Title,
                        LastModified = createReminder.LastModified,
                    }
                };
            }
            return new BaseResponse<ReminderResponseModel>
            {
                Message = "Input fields Carefully",
                Success = false
            };
        }

        public async Task DeleteReminder(ReminderRequestModel request)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ReminderResponseModel>> GetAllDueReminders()
        {
            var dueReminders = await _reminderRepository.GetDueRemindersAsync();
            if(dueReminders.Any(x => x.IsDeleted == false))
            {
                return dueReminders.Select(x => new ReminderResponseModel
                {
                    DateCreated = x.DateCreated,
                    Description = x.Description,
                    DueDate = x.DueDate,
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    IsSent = x.IsSent,
                    Title = x.Title,
                    UserId = x.UserId
                });
            }
            return null;
        }

        public async Task<BaseResponse<IEnumerable<ReminderResponseModel>>> GetAllReminders()
        {
            var allReminders = await _reminderRepository.GetAllReminders(_context.UserId);
            if (allReminders.Any())
                return new BaseResponse<IEnumerable<ReminderResponseModel>>
                {
                    Message = "All Reminders",
                    Success = true,
                    Data = allReminders.Select(x => new ReminderResponseModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        DueDate = x.DueDate,
                        ImageUrl = x.ImageUrl,
                        DateCreated = x.DateCreated,
                        LastModified = x.LastModified,
                        
                    })
                };
            return new BaseResponse<IEnumerable<ReminderResponseModel>>
            {
                Message = "No Records Found",
                Success = false
            };

        }

        public async Task<BaseResponse<ReminderResponseModel>> GetReminders(ReminderRequestModel request)
        {
            var reminder = await _reminderRepository.GetReminderAsync(request.Title,request.Description);

            if (reminder is null || reminder.UserId != _context.UserId || reminder.IsDeleted)
            {
                return new BaseResponse<ReminderResponseModel>
                {
                    Success = false,
                    Message = "Reminder not found or not accessible."
                };
            }

            return new BaseResponse<ReminderResponseModel>
            {
                Success = true,
                Message = "Reminder retrieved successfully.",
                Data = new ReminderResponseModel
                {
                    Id = reminder.Id,
                    Title = reminder.Title,
                    Description = reminder.Description,
                    DueDate = reminder.DueDate,
                    ImageUrl = reminder.ImageUrl,
                    DateCreated = reminder.DateCreated,
                    LastModified = reminder.LastModified,
                    IsSent = reminder.IsSent,
                    IsDeleted = reminder.IsDeleted
                }
            };
        }



        public async Task<BaseResponse<ReminderResponseModel>> RestoreReminder(ReminderRequestModel request)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SendReminderAsync(ReminderEmailRequestModel request)
        {
            var reminder = await _reminderRepository.GetReminderAsync(request.ReminderId);
            if(reminder == null)
            {
                return false;
            }
            var emailSent = await _emailService.SendReminderEmail(request);

            if (emailSent)
            {
                reminder.IsSent = true;
                await _reminderRepository.UpdateReminderAsync(reminder.Id); 
            }

            return emailSent;
        }
        public async Task<BaseResponse<ReminderResponseModel>> UpdateReminder(ReminderUpdateModel request)
        {
            var response = new BaseResponse<ReminderResponseModel>();
            var reminder = await _reminderRepository.GetReminderAsync(request.ReminderId);
            if (reminder == null)
            {
                response.Success = false;
                response.Message = "Reminder not found.";
                return response;
            }

            if (reminder.LastModified != reminder.DateCreated)
            {
                response.Success = false;
                response.Message = "Reminder has already been updated and can no longer be modified.";
                response.Data = new ReminderResponseModel
                {
                    Id = reminder.Id,
                    Title = reminder.Title,
                    Description = reminder.Description,
                    DueDate = reminder.DueDate,
                    ImageUrl = reminder.ImageUrl,
                    DateCreated = reminder.DateCreated,
                    LastModified = reminder.LastModified
                };
                return response;
            }

            if (DateTime.Now - reminder.DateCreated > TimeSpan.FromMinutes(15))
            {
                response.Success = false;
                response.Message = "Update period expired. You can only update the reminder within 15 minutes of creation.";
                response.Data = new ReminderResponseModel
                {
                    Id = reminder.Id,
                    Title = reminder.Title,
                    Description = reminder.Description,
                    DueDate = reminder.DueDate,
                    ImageUrl = reminder.ImageUrl,
                    DateCreated = reminder.DateCreated,
                    LastModified = reminder.LastModified
                };
                return response;
            }

            reminder.Title = string.IsNullOrWhiteSpace(request.Title) ? reminder.Title : request.Title;
            reminder.Description = string.IsNullOrWhiteSpace(request.Description) ? reminder.Description : request.Description;
            if (request.DueDate > DateTime.Now)
                reminder.DueDate = request.DueDate;
            if (request.Image != null)
                reminder.ImageUrl = await _fileService.UploadImage(request.Image);
            reminder.LastModified = DateTime.Now;

            await _reminderRepository.UpdateReminderAsync(reminder.Id);

            response.Success = true;
            response.Message = "Reminder updated successfully.";
            response.Data = new ReminderResponseModel
            {
                Id = reminder.Id,
                Title = reminder.Title,
                Description = reminder.Description,
                DueDate = reminder.DueDate,
                ImageUrl = reminder.ImageUrl,
                DateCreated = reminder.DateCreated,
                LastModified = reminder.LastModified,
                
            };

            return response;
        }


    }
}
