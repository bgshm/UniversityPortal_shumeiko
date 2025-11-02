namespace UniversityPortal_shumeiko.Services
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(IFormFile file, string blobName = null);
        Task DeleteFileAsync(string blobName);
    }
}
