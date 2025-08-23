namespace Booking_WEB.Services.Storage
{
    public interface IStorageService
    {
        byte[] GetItemBytes(String itemName);
        String TryGetMimeType(String itemName);
        String SaveItem(IFormFile formFile);
        Task<String> SaveItemAsync(IFormFile formFile);
    }
}
