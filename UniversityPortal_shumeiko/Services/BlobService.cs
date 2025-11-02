using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace UniversityPortal_shumeiko.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobService(IConfiguration configuration)
        {
            // Получаем Connection String из конфигурации
            _blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("BlobStorage"));
            _containerName = configuration.GetValue<string>("BlobStorage:ContainerName");
        }

        // Метод для ЗАГРУЗКИ
        public async Task<string> UploadFileAsync(IFormFile file, string blobName = null)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            // Гарантируем, что контейнер существует (на всякий случай)
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Генерируем уникальное имя файла, если оно не предоставлено
            if (string.IsNullOrEmpty(blobName))
            {
                blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            }

            var blobClient = containerClient.GetBlobClient(blobName);

            await using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }

            // Возвращаем ПУБЛИЧНЫЙ URL-адрес загруженного файла
            return blobClient.Uri.ToString();
        }

        // Метод для УДАЛЕНИЯ (если нужно)
        public async Task DeleteFileAsync(string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }
    }
}
