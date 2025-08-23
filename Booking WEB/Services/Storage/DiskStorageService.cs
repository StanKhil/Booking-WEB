using Microsoft.Extensions.Options;

namespace Booking_WEB.Services.Storage
{
    public class DiskStorageService : IStorageService
    {
        private readonly String? basePath;

        public DiskStorageService(IOptions<StorageOptions> options)
        {
            basePath = options.Value.StoragePath;
        }

        public byte[] GetItemBytes(string itemName)
        {
            String path = Path.Combine(basePath, itemName);
            if (File.Exists(path))
                return File.ReadAllBytes(path);
            throw new FileNotFoundException($"File '{itemName}' not found in storage.");
        }

        public string TryGetMimeType(string itemName)
        {
            String ext = GetFileExtension(itemName);
            return ext switch
            {
                ".jpg" => "image/jpeg",
                ".png" => "image/png",
                ".bmp" => "image/bmp",
                _ => throw new ArgumentException($"Unsupported exception '{ext}'")
            };
        }

        public string SaveItem(IFormFile formFile)
        {
            String ext = GetFileExtension(formFile.FileName);

            String savedName = Guid.NewGuid() + ext;
            String path = Path.Combine(basePath, savedName);

            using Stream stream = new StreamWriter(path).BaseStream;
            formFile.CopyTo(stream);

            return savedName;
        }

        public async Task<string> SaveItemAsync(IFormFile formFile)
        {
            String ext = GetFileExtension(formFile.FileName);

            String savedName = Guid.NewGuid() + ext;
            String path = Path.Combine(basePath, savedName);

            using Stream stream = new StreamWriter(path).BaseStream;
            await formFile.CopyToAsync(stream);

            return savedName;
        }

        private String GetFileExtension(String filename)
        {
            int dotIndex = filename.LastIndexOf(".");
            if (dotIndex < 0)
            {
                throw new ArgumentException("File name must have an extension");
            }
            return filename[dotIndex..];
        }
    }
}
