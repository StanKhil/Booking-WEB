using System.Text;

namespace Booking_WEB.Services.Kdf
{
    public class PbKdfService : IKdfService
    {
        public string Dk(string password, string salt)
        {
            int c = 3;
            int dkLength = 20;
            string t = password + salt;
            for (int i = 0; i < c; i++)
                t = Hash(t);

            return t[0..dkLength];
        }

        private static string Hash(string input)
        {
            return Convert.ToHexString(System.Security.Cryptography.SHA1.HashData(Encoding.UTF8.GetBytes(input)));
        }
    }
}
