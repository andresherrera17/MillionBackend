using Microsoft.AspNetCore.Http;


namespace Million.PropertiesApi.Business.Interfaces
{
    public interface IBlobStorageService
    {
        public Task<List<string>> UploadFilesAsync(IEnumerable<IFormFile> files, CancellationToken ct = default);
        public  Task<string?> UploadFileAsync(IFormFile? file, CancellationToken ct = default);
    }
}
