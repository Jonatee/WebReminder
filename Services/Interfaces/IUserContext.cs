﻿namespace WebReminder.Services.Interfaces
{
    public interface IUserContext
    {
         Guid UserId { get; }
         string UserIpAddress { get; }
    }
}
