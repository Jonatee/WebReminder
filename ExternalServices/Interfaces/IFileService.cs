namespace WebReminder.ExternalServices.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadImage(IFormFile file);
    }
}
