namespace Booking_WEB.Models.Rest
{
    public class RestStatus
    {
        public bool IsOk { get; set; } = true;
        public int Code { get; set; } = 200;
        public String Phrase { get; set; } = "OK";

        public static readonly RestStatus RestStatus403 = new()
        {
            Code = 403,
            IsOk = false,
            Phrase = "Forbidden"
        };

        public static readonly RestStatus RestStatus404 = new()
        {
            Code = 404,
            IsOk = false,
            Phrase = "NotFound"
        };

        public static readonly RestStatus RestStatus401 = new()
        {
            Code = 401,
            IsOk = false,
            Phrase = "UnAuthorized"
        };

        public static readonly RestStatus RestStatus400 = new()
        {
            Code = 400,
            IsOk = false,
            Phrase = "Bad Request"
        };

        public static readonly RestStatus RestStatus500 = new()
        {
            Code = 500,
            IsOk = false,
            Phrase = "Internal Error"
        };

        public static readonly RestStatus RestStatus201 = new()
        {
            Code = 201,
            IsOk = true,
            Phrase = "Created"
        };

        public static readonly RestStatus RestStatus200 = new()
        {
            Code = 200,
            IsOk = true,
            Phrase = "Ok"
        };

        public static readonly RestStatus RestStatus409 = new()
        {
            Code = 409,
            IsOk = false,
            Phrase = "Conflict"
        };
    }
}
