namespace Booking_WEB.Services.Kdf
{
    public interface IKdfService
    {
        string Dk(string password, string salt);
    }
}
